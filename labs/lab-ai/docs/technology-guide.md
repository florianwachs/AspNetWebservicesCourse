# Technology Guide — Conference Pulse

Conference Pulse demonstrates five Microsoft AI technologies for .NET working together in a single application. This guide walks through each technology, showing exactly how it's used with file references and code patterns.

> **Tip:** This document is meant to be read alongside the source code. Every section includes "Where to look" references — open those files to see the full implementation.

---

## 1. Microsoft.Extensions.AI

**What it is:** Provider-agnostic abstractions for AI services. Think of it as the `ILogger` of AI — your code programs against `IChatClient` and `IEmbeddingGenerator`, not against OpenAI or Azure directly.

**Why it matters:** Swap providers without changing application code. Add cross-cutting concerns (telemetry, logging, function invocation) via middleware, just like an ASP.NET Core pipeline.

### Registration

In `src/ConferenceAssistant.Web/Program.cs` (lines 62–69):

```csharp
var openaiBuilder = builder.AddAzureOpenAIClient("openai");

openaiBuilder.AddChatClient("chat")
    .UseFunctionInvocation()   // Agents can call tools automatically
    .UseOpenTelemetry()        // Distributed tracing for every LLM call
    .UseLogging();             // Request/response logging

openaiBuilder.AddEmbeddingGenerator("embedding");
```

The Aspire `AddAzureOpenAIClient` call injects connection info from the AppHost. The `ChatClientBuilder` wraps the raw client with a middleware pipeline — every `IChatClient` call in the app flows through function invocation, OpenTelemetry, and logging automatically.

### Key abstractions used

| Abstraction | Purpose | Where used |
|-------------|---------|------------|
| `IChatClient` | Every LLM call — agents, Q&A, insights, poll generation | Injected across services and workflows |
| `IEmbeddingGenerator<string, Embedding<float>>` | Generate vector embeddings for ingestion and semantic search | `SemanticSearchService`, `VectorStoreWriter` |
| `ChatOptions` | Per-call configuration — system prompt, tools, temperature | Workflow classes, service methods |
| `AIFunctionFactory` | Create typed tool definitions from .NET methods | `AgentTools.cs` |
| `ChatMessage` / `ChatRole` | Structured conversation history | All workflows and services |

### Tool creation with AIFunctionFactory

In `src/ConferenceAssistant.Agents/Tools/AgentTools.cs`, each agent tool is created from a regular C# method:

```csharp
SearchKnowledge = AIFunctionFactory.Create(SearchKnowledgeCore, new AIFunctionFactoryOptions
{
    Name = nameof(SearchKnowledge),
    Description = "Search the session knowledge base for content related to the query"
});
```

The factory inspects method parameters and return types to generate a tool schema that LLMs can invoke. No manual JSON schema authoring required.

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Web/Program.cs`](../src/ConferenceAssistant.Web/Program.cs) | Registration, middleware pipeline (lines 62–69) |
| [`src/ConferenceAssistant.Agents/Tools/AgentTools.cs`](../src/ConferenceAssistant.Agents/Tools/AgentTools.cs) | `AIFunctionFactory.Create` for all 8 tools |
| [`src/ConferenceAssistant.Web/Services/QuestionAnsweringService.cs`](../src/ConferenceAssistant.Web/Services/QuestionAnsweringService.cs) | `IChatClient` for Q&A with multi-source context |
| [`src/ConferenceAssistant.Web/Services/InsightGenerationService.cs`](../src/ConferenceAssistant.Web/Services/InsightGenerationService.cs) | `IChatClient` for poll analysis, topic summaries, trend detection |

**Official docs:** [Microsoft.Extensions.AI on MS Learn](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)

---

## 2. Microsoft.Extensions.DataIngestion

**What it is:** A pipeline framework for ingesting content into AI-ready formats — read documents, chunk them intelligently, enrich with metadata, and write to a vector store.

**Why it matters:** RAG (Retrieval-Augmented Generation) requires turning raw content into searchable, embedded chunks. DataIngestion provides a structured pipeline so you don't hand-roll this every time.

### The ingestion pipeline

In `src/ConferenceAssistant.Ingestion/Services/IngestionService.cs` (lines 151–177), the GitHub content ingestion pipeline is assembled:

```csharp
// Reader: parse markdown files into documents
IngestionDocumentReader reader = new MarkdownReader();

// Chunker: split on headers with a token budget
var tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");
var chunkerOptions = new IngestionChunkerOptions(tokenizer)
{
    MaxTokensPerChunk = 500,
    OverlapTokens = 50
};
IngestionChunker<string> chunker = new HeaderChunker(chunkerOptions);

// Writer: store chunks with embeddings in Qdrant
using var writer = new VectorStoreWriter<string>(
    _searchService.VectorStore,
    dimensionCount: 1536,
    new VectorStoreWriterOptions
    {
        CollectionName = "conference_knowledge",
        IncrementalIngestion = true   // Only re-embeds changed chunks
    });

// Assemble the pipeline with enrichers
using IngestionPipeline<string> pipeline = new(reader, chunker, writer,
    new IngestionPipelineOptions(), _loggerFactory)
{
    ChunkProcessors =
    {
        new SummaryEnricher(enricherOptions),      // AI-generated summaries per chunk
        new KeywordEnricher(enricherOptions, ...),  // AI-extracted keywords
        frontMatterProcessor                        // YAML front matter metadata
    }
};
```

### Pipeline components

| Component | Class | Purpose |
|-----------|-------|---------|
| **Reader** | `MarkdownReader` | Parses Markdown files into documents |
| **Chunker** | `HeaderChunker` | Splits on headings; 500 tokens max, 50-token overlap |
| **Enricher** | `SummaryEnricher` | LLM-generated summary for each chunk |
| **Enricher** | `KeywordEnricher` | LLM-extracted keywords for each chunk |
| **Enricher** | `FrontMatterChunkProcessor` | Injects YAML front matter metadata (technologies, category) |
| **Writer** | `VectorStoreWriter<string>` | Embeds + upserts chunks into Qdrant |

### What gets ingested

The knowledge base grows throughout the session via direct upserts (not the full pipeline):

| Content | When | Source tag | Method |
|---------|------|------------|--------|
| Session outline | Startup | `outline` | `UpsertAsync` |
| Slide content | Startup | `slide` | `UpsertAsync` |
| Poll results | On poll close | `response` | `IngestResponseAsync` |
| Audience questions | On submit | `question` | `IngestQuestionAsync` |
| Q&A pairs | On answer | `qa` | `IngestExternalContentAsync` |
| AI insights | On generation | `insight` | `IngestInsightAsync` |
| GitHub content | On import | `github:owner/repo/branch` | `IngestGitHubRepoAsync` (full pipeline) |
| Session summary | On session end | `session-summary` | `IngestSessionSummaryAsync` |

### Change detection for GitHub content

The pipeline uses SHA256 content hashing to skip unchanged files:

```csharp
var hash = ComputeHash(content);
var existing = await _tracker.GetRecordAsync(docId, source);
if (existing is not null && existing.ContentHash == hash
    && existing.Status == IngestionStatus.Completed)
{
    _logger.LogInformation("Skipping unchanged document: {DocId}", docId);
    continue;
}
```

Each document's ingestion status (Pending → Completed/Failed) is tracked via `IIngestionTracker`, backed by PostgreSQL.

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Ingestion/Services/IngestionService.cs`](../src/ConferenceAssistant.Ingestion/Services/IngestionService.cs) | Full pipeline assembly, all `Ingest*` methods |
| [`src/ConferenceAssistant.Ingestion/Readers/GitHubRepoReader.cs`](../src/ConferenceAssistant.Ingestion/Readers/GitHubRepoReader.cs) | GitHub content download with rate limit handling |
| [`src/ConferenceAssistant.Ingestion/Enrichers/FrontMatterEnricher.cs`](../src/ConferenceAssistant.Ingestion/Enrichers/FrontMatterEnricher.cs) | YAML front matter → chunk metadata |

**Official docs:** [Data Ingestion on MS Learn](https://learn.microsoft.com/dotnet/ai/data-ingestion-overview)

---

## 3. Microsoft.Extensions.VectorData

**What it is:** Abstractions for vector storage and semantic search — define your schema, embed content, search by similarity. Provider-agnostic, like the rest of Microsoft.Extensions.AI.

**Why it matters:** Semantic search is what makes RAG work. Instead of keyword matching, you search by meaning. VectorData gives you a clean API over any vector database.

### Vector store setup

Conference Pulse uses Qdrant as its vector backend (via `Microsoft.SemanticKernel.Connectors.Qdrant`).

In `src/ConferenceAssistant.Ingestion/Services/SemanticSearchService.cs`:

```csharp
public class SemanticSearchService : ISemanticSearchService
{
    private const string CollectionName = "conference_knowledge";
    private const int Dimensions = 1536;

    public SemanticSearchService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        QdrantClient qdrantClient)
    {
        _storeOptions = new QdrantVectorStoreOptions
        {
            EmbeddingGenerator = embeddingGenerator
        };
        _vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: false, _storeOptions);
    }
}
```

The `EmbeddingGenerator` is passed into the store options so that string content is automatically embedded on upsert — no manual embedding step needed.

### Collection schema

The collection uses a dynamic dictionary schema to match what `VectorStoreWriter` produces:

```csharp
var definition = new VectorStoreCollectionDefinition();
definition.Properties.Add(new VectorStoreKeyProperty("key", typeof(Guid)));
definition.Properties.Add(new VectorStoreVectorProperty("embedding", typeof(string), Dimensions)
{
    DistanceFunction = DistanceFunction.CosineSimilarity
});
definition.Properties.Add(new VectorStoreDataProperty("content", typeof(string)));
definition.Properties.Add(new VectorStoreDataProperty("context", typeof(string)));
definition.Properties.Add(new VectorStoreDataProperty("documentid", typeof(string)));
definition.Properties.Add(new VectorStoreDataProperty("source", typeof(string)));
```

Note that `embedding` has type `string` — the auto-embedding feature converts the string content into a float vector transparently.

### Store and search

**Upsert** (auto-embeds the content string):
```csharp
var record = new Dictionary<string, object?>
{
    ["key"] = Guid.NewGuid(),
    ["content"] = content,
    ["embedding"] = content,      // auto-embedded via IEmbeddingGenerator
    ["context"] = "",
    ["documentid"] = documentId,
    ["source"] = source
};
await collection.UpsertAsync(record);
```

**Search** (auto-embeds the query string):
```csharp
var results = collection.SearchAsync(query, topK);
await foreach (var result in results)
{
    var content = result.Record["content"] as string;
    var source  = result.Record["source"] as string;
    records.Add(new SearchResult(content, context, source, docId));
}
```

Both operations pass strings that get auto-embedded — no need to call `IEmbeddingGenerator` explicitly.

### The "Snowball Effect"

As the session progresses, the vector store grows richer, and agents produce better results:

| Stage | Knowledge in vector store | Agent quality |
|-------|--------------------------|---------------|
| **Startup** | Session outline + slides | Basic topic context |
| **First polls** | + poll results + "Other" responses | Audience understanding |
| **Mid-session** | + AI insights + Q&A pairs | Trend awareness |
| **Late session** | + GitHub docs via MCP import | Maximum context |
| **Session end** | + comprehensive summary | Full knowledge base |

Each new piece of knowledge is immediately searchable by all agents and the MCP server. The more the audience participates, the smarter the system becomes.

### Infrastructure

In `src/ConferenceAssistant.AppHost/AppHost.cs` (lines 28–31):

```csharp
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()                         // Persists vectors across restarts
    .WithLifetime(ContainerLifetime.Persistent);
```

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Ingestion/Services/SemanticSearchService.cs`](../src/ConferenceAssistant.Ingestion/Services/SemanticSearchService.cs) | `QdrantVectorStore`, schema definition, `SearchAsync`, `UpsertAsync` |
| [`src/ConferenceAssistant.AppHost/AppHost.cs`](../src/ConferenceAssistant.AppHost/AppHost.cs) | Qdrant infrastructure setup (lines 28–31) |

**Official docs:** [VectorData on MS Learn](https://learn.microsoft.com/dotnet/ai/vector-data-overview)

---

## 4. Microsoft Agent Framework

**What it is:** A framework for building AI agents with tools, system prompts, and multi-agent workflows.

**Package:** `Microsoft.Agents.AI`

**Why it matters:** Agents go beyond simple prompt-response — they can call tools, reason about results, and collaborate with other agents. The framework provides `ChatClientAgent`, workflow builders, and execution infrastructure.

### Agents in Conference Pulse

| Agent | Role | Tools |
|-------|------|-------|
| **Survey Architect** | Generates polls informed by context, previous polls, and audience questions | GetCurrentTopic, SearchKnowledge, GetAudienceQuestions, GetAllPollResults, GetAllInsights, CreatePoll |
| **Response Analyst** | Analyzes poll results, identifies trends, generates actionable insights | GetPollResults, SearchKnowledge, GetAllPollResults, SaveInsight |
| **Knowledge Curator** | Synthesizes information from the knowledge base, identifies gaps | SearchKnowledge, SaveInsight |

Each agent has a detailed system prompt in `AgentDefinitions.cs` that defines its persona, tool-use strategy, and output expectations.

### Workflow pattern 1: Single-agent with tools

`PollGenerationWorkflow` gives the Survey Architect all its tools and lets it reason:

```csharp
public class PollGenerationWorkflow(IChatClient chatClient, AgentTools tools)
{
    public async Task<string> ExecuteAsync(string topicId)
    {
        var options = new ChatOptions
        {
            Tools = [tools.GetCurrentTopic, tools.SearchKnowledge,
                     tools.GetAudienceQuestions, tools.GetAllPollResults,
                     tools.GetAllInsights, tools.CreatePoll]
        };

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, AgentDefinitions.SurveyArchitectInstructions),
            new(ChatRole.User, $"Generate an engaging poll for topic ID: {topicId}...")
        };

        var response = await chatClient.GetResponseAsync(messages, options);
        return response.Text ?? "Unable to generate poll.";
    }
}
```

The `UseFunctionInvocation()` middleware (registered in `Program.cs`) handles the tool call loop automatically — the LLM calls tools, gets results, and continues reasoning until it produces a final answer.

### Workflow pattern 2: Fan-out / fan-in

`SessionSummaryWorkflow` uses the Microsoft Agent Framework's workflow builder for concurrent multi-agent execution:

```csharp
// Three agents analyze concurrently (fan-out)
ChatClientAgent pollAnalyst = new(chatClient,
    name: "PollAnalyst",
    instructions: "Use GetAllPollResults to summarize poll trends...",
    tools: [tools.GetAllPollResults]);

ChatClientAgent questionAnalyst = new(chatClient,
    name: "QuestionAnalyst",
    instructions: "Use GetAudienceQuestions to identify themes...",
    tools: [tools.GetAudienceQuestions]);

ChatClientAgent insightAnalyst = new(chatClient,
    name: "InsightAnalyst",
    instructions: "Use GetAllInsights and SearchKnowledge...",
    tools: [tools.GetAllInsights, tools.SearchKnowledge]);

// Synthesizer merges all outputs (fan-in)
ChatClientAgent synthesizer = new(chatClient,
    name: "Synthesizer",
    instructions: "Synthesize analyses into a 4-6 paragraph summary...");

// Build: concurrent(analysts) → sequential(synthesizer)
var analysisWorkflow = AgentWorkflowBuilder.BuildConcurrent(
    [pollAnalyst, questionAnalyst, insightAnalyst], MergeAgentOutputs);

var composedWorkflow = new WorkflowBuilder(analysisExec)
    .WithName("SessionSummaryPipeline")
    .BindExecutor(synthExec)
    .AddEdge(analysisExec, synthExec)
    .WithOutputFrom([synthExec])
    .Build();

// Execute the full pipeline
var run = await InProcessExecution.Default.RunAsync(
    composedWorkflow,
    "Analyze the conference session data...");
```

Key types from `Microsoft.Agents.AI.Workflows`: `AgentWorkflowBuilder`, `WorkflowBuilder`, `SubworkflowBinding`, `InProcessExecution`.

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Agents/Definitions/AgentDefinitions.cs`](../src/ConferenceAssistant.Agents/Definitions/AgentDefinitions.cs) | System prompts for all agents |
| [`src/ConferenceAssistant.Agents/Workflows/PollGenerationWorkflow.cs`](../src/ConferenceAssistant.Agents/Workflows/PollGenerationWorkflow.cs) | Single-agent tool-use pattern |
| [`src/ConferenceAssistant.Agents/Workflows/ResponseAnalysisWorkflow.cs`](../src/ConferenceAssistant.Agents/Workflows/ResponseAnalysisWorkflow.cs) | Single-agent analysis pattern |
| [`src/ConferenceAssistant.Agents/Workflows/SessionSummaryWorkflow.cs`](../src/ConferenceAssistant.Agents/Workflows/SessionSummaryWorkflow.cs) | Fan-out/fan-in multi-agent workflow |
| [`src/ConferenceAssistant.Agents/Tools/AgentTools.cs`](../src/ConferenceAssistant.Agents/Tools/AgentTools.cs) | All 8 tool definitions via `AIFunctionFactory` |

**Official docs:** [AI Agents on MS Learn](https://learn.microsoft.com/dotnet/ai/agents-overview)

---

## 5. Model Context Protocol (MCP)

**What it is:** An open protocol for AI systems to discover and use tools across applications. Conference Pulse is both an MCP **server** (exposing its tools to external clients) and an MCP **client** (connecting to external knowledge sources).

**Packages:** `ModelContextProtocol` and `ModelContextProtocol.AspNetCore`

### Conference Pulse as MCP Server

The app exposes 10 tools via Streamable HTTP at `/mcp`, making its data and capabilities available to GitHub Copilot, Claude Desktop, or any MCP-compatible client.

**Registration** in `src/ConferenceAssistant.Web/Program.cs` (lines 94–104):

```csharp
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "ConferencePulse",
            Version = "1.0.0"
        };
    })
    .WithToolsFromAssembly(typeof(ConferenceAssistant.Mcp.Tools.ConferenceTools).Assembly)
    .WithHttpTransport();

// Later, map the endpoint:
app.MapMcp("/mcp");
```

`WithToolsFromAssembly` automatically discovers classes decorated with `[McpServerToolType]` and methods with `[McpServerTool]`.

### MCP Server tools

| Tool | Description | Read-Only |
|------|-------------|-----------|
| `get_session_status` | Session title, status, active topic, all topics with status markers | Yes |
| `get_active_poll` | Current poll with question, options, live vote counts and percentages | Yes |
| `get_poll_results` | Detailed results for a specific poll with bar chart visualization | Yes |
| `search_session_knowledge` | Semantic search over the vector knowledge base | Yes |
| `get_audience_questions` | Top audience questions ranked by upvotes, with answer badges | Yes |
| `get_topic_insights` | AI-generated insights for a specific topic | Yes |
| `get_all_insights` | All session insights grouped by type | Yes |
| `generate_session_summary` | Comprehensive multi-part session summary | Yes |
| `search_knowledge` | Search knowledge base by query string | Yes |
| `get_knowledge_stats` | Knowledge base record count | Yes |

All tools return Markdown-formatted responses, making them human-readable when used in chat interfaces.

### Conference Pulse as MCP Client

The app connects to two external MCP servers to enrich its knowledge base:

In `src/ConferenceAssistant.Mcp/Clients/McpContentClient.cs`:

```csharp
// Microsoft Learn — official .NET and Azure documentation
var learnTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp
}, loggerFactory);
_learnClient = await McpClient.CreateAsync(learnTransport, null, loggerFactory, ct);

// DeepWiki — GitHub repository knowledge
var deepWikiTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://mcp.deepwiki.com/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp
}, loggerFactory);
_deepWikiClient = await McpClient.CreateAsync(deepWikiTransport, null, loggerFactory, ct);
```

These clients are used by `QuestionAnsweringService` to assemble multi-source context:

1. **Local knowledge base** — semantic search over session data (always available)
2. **Microsoft Learn** — official documentation via `microsoft_docs_search` tool call
3. **DeepWiki** — GitHub repository knowledge via `ask_question` tool call

Connection failures are handled gracefully — if an external server is unavailable, the service falls back to local context only.

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Mcp/Tools/ConferenceTools.cs`](../src/ConferenceAssistant.Mcp/Tools/ConferenceTools.cs) | 8 MCP tools with `[McpServerTool]` attributes |
| [`src/ConferenceAssistant.Mcp/Tools/KnowledgeTools.cs`](../src/ConferenceAssistant.Mcp/Tools/KnowledgeTools.cs) | 2 knowledge-focused MCP tools |
| [`src/ConferenceAssistant.Mcp/Clients/McpContentClient.cs`](../src/ConferenceAssistant.Mcp/Clients/McpContentClient.cs) | External MCP client connections |
| [`src/ConferenceAssistant.Web/Services/QuestionAnsweringService.cs`](../src/ConferenceAssistant.Web/Services/QuestionAnsweringService.cs) | Multi-source context assembly using MCP client |

**Official docs:** [Model Context Protocol](https://modelcontextprotocol.io/)

---

## Cross-Cutting: The Event System

All five technologies are wired together through the `SessionContext` event system. Events fired by user actions trigger chains of AI processing.

### Event flow

```
QuestionReceived → IsQuestionSafeAsync (content safety via IChatClient)
                 → GenerateAiAnswerTextAsync (multi-source: KB + MS Learn + DeepWiki)
                 → IngestQuestionAsync (vector store grows)
                 → GenerateQuestionInsightsAsync (debounced trend detection)

PollClosed → IngestResponseAsync (poll results → vector store)
           → GeneratePollInsightsAsync (ResponseAnalyst agent)
           → IngestInsightAsync (insights → vector store)

TopicCompleted → GenerateTopicInsightsAsync (topic summary + gap detection)

SessionEnded → SessionSummaryWorkflow (fan-out/fan-in multi-agent)
             → IngestSessionSummaryAsync (summary → vector store)
```

### How it's wired

In `src/ConferenceAssistant.Web/Program.cs` (lines 183–352), the `SessionCreated` event handler subscribes to all domain events:

```csharp
sessionManager.SessionCreated += ctx =>
{
    // Auto-answer questions with AI
    ctx.QuestionReceived += q =>
    {
        _ = Task.Run(async () =>
        {
            if (!await questionAnswering.IsQuestionSafeAsync(q.Text))
            {
                ctx.MarkQuestionUnsafe(q.Id);
                return;
            }
            var answer = await questionAnswering.GenerateAiAnswerTextAsync(q.Text, q.TopicId);
            if (!string.IsNullOrWhiteSpace(answer))
                ctx.AnswerQuestion(q.Id, answer, isAiGenerated: true, authorLabel: "AI");
        });
    };

    // Ingest + analyze when polls close
    ctx.PollClosed += poll =>
    {
        _ = Task.Run(async () =>
        {
            var results = ctx.GetPollResults(poll.Id);
            await ingestionService.IngestResponseAsync(poll.Id, poll.TopicId ?? "", ...);
            await insightGen.GeneratePollInsightsAsync(poll.Id, ctx.Session.SessionCode);
        });
    };

    // Generate session summary on end
    ctx.SessionEnded += () =>
    {
        _ = Task.Run(async () =>
        {
            var summary = await workflow.ExecuteAsync();
            await ingestionService.IngestSessionSummaryAsync(summary);
        });
    };
};
```

Every mutation is also persisted to PostgreSQL via `ISessionPersistenceService` — fire-and-forget `Task.Run` calls ensure the UI stays responsive.

### Where to look

| File | What to see |
|------|-------------|
| [`src/ConferenceAssistant.Web/Program.cs`](../src/ConferenceAssistant.Web/Program.cs) | Event wiring (lines 183–352) |
| [`src/ConferenceAssistant.Core/Models/SessionContext.cs`](../src/ConferenceAssistant.Core/Models/SessionContext.cs) | Event definitions, thread-safe collections, state management |

---

## Infrastructure: .NET Aspire

All services are orchestrated via .NET Aspire in `src/ConferenceAssistant.AppHost/AppHost.cs`:

```csharp
// Azure OpenAI — reference an existing resource
var openai = builder.AddAzureOpenAI("openai")
    .RunAsExisting(azOpenAiName, azOpenAiRg);
openai.AddDeployment("chat", "gpt-4o", "2024-08-06");
openai.AddDeployment("embedding", "text-embedding-3-small", "1");

// PostgreSQL — EF Core persistence
var postgres = builder.AddPostgres("postgres")
    .WithPgWeb()             // Web UI for debugging
    .WithDataVolume();
var conferenceDb = postgres.AddDatabase("conferencedb");

// Qdrant — vector/embedding storage
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Web app with all dependencies
var web = builder.AddProject<Projects.ConferenceAssistant_Web>("web")
    .WithReference(openai)
    .WithReference(conferenceDb)
    .WithReference(qdrant)
    .WaitFor(openai)
    .WaitFor(conferenceDb)
    .WaitFor(qdrant);

// Dev tunnel for attendee access
builder.AddDevTunnel("conference-tunnel")
    .WithReference(web)
    .WithAnonymousAccess();
```

Aspire provides the dashboard for monitoring all resources, OpenTelemetry integration for distributed tracing across LLM calls, and connection string management.

---

## Further Reading

| Technology | Official Documentation |
|-----------|----------------------|
| Microsoft.Extensions.AI | [MS Learn](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai) |
| DataIngestion | [MS Learn](https://learn.microsoft.com/dotnet/ai/data-ingestion-overview) |
| VectorData | [MS Learn](https://learn.microsoft.com/dotnet/ai/vector-data-overview) |
| Agent Framework | [MS Learn](https://learn.microsoft.com/dotnet/ai/agents-overview) |
| Model Context Protocol | [modelcontextprotocol.io](https://modelcontextprotocol.io/) |
| .NET Aspire | [MS Learn](https://learn.microsoft.com/dotnet/aspire/) |
