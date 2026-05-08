<!-- layout: centered -->

# The Microsoft AI Stack for .NET

## A Living Presentation

<!-- speaker:
Welcome everyone! This session is different — there are no traditional slides.
This application IS the presentation. Everything you see is powered by AI agents
built with the Microsoft AI stack for .NET.

Timing: You have 60 minutes total. Keep the intro to ~3 minutes.
-->

---

<!-- layout: centered -->

## Scan to Join

### 📱 Your phone is your remote control

- Vote on polls
- Ask questions
- Shape this session in real-time

<!-- speaker:
Point at the QR code on the display screen.
Say: "Take out your phones and scan the QR code. You'll be voting on polls,
asking questions, and literally shaping the content of this session live."
Wait 30-60 seconds for people to join. Watch the join count on /presenter.
-->

---

<!-- topic: meai -->
<!-- layout: centered -->

# Microsoft.Extensions.AI

## The Common Language

<!-- speaker:
Transition: "Let's start with the foundation. Everything in the Microsoft AI stack
speaks through one set of abstractions."
15 minutes for this segment.
Ask: "How many of you have swapped an AI provider mid-project and had to rewrite
your integration layer? That pain ends today."
-->

---

## Key Abstractions

- **IChatClient** — one interface for any LLM provider
- **IEmbeddingGenerator** — embeddings for search & ingestion
- **ChatClientBuilder** — composable middleware pipeline
- **AIFunctionFactory** — turn any .NET method into an AI tool

<!-- speaker:
Walk through each abstraction briefly.
IChatClient: "Your code talks to IChatClient, not to Azure OpenAI or Ollama directly."
IEmbeddingGenerator: "Same pattern — provider-agnostic embeddings."
ChatClientBuilder: "Think of it like ASP.NET middleware but for AI calls."
AIFunctionFactory: "This is the bridge between AI reasoning and your code."
-->

---

## The Middleware Pipeline

```csharp
var openaiBuilder = builder.AddAzureOpenAIClient("openai");

openaiBuilder.AddChatClient("chat")
    .UseFunctionInvocation()
    .UseOpenTelemetry()
    .UseLogging();

openaiBuilder.AddEmbeddingGenerator("embedding");
```

<!-- speaker:
Open src/ConferenceAssistant.Web/Program.cs and show lines 32-42.
Say: "Three lines of middleware. Function invocation so agents can call tools.
OpenTelemetry so every LLM call shows up in your traces. Logging for debugging.
And the embedding generator is registered separately for vector search."
Emphasize: This is real code from THIS application.
-->

---

## Turn Any Method into a Tool

```csharp
AIFunctionFactory.Create(
    async (
        [Description("The topic ID")] string topicId,
        [Description("The poll question")] string question,
        [Description("Answer options")] string[] options
    ) =>
    {
        var poll = await pollService.CreatePollAsync(
            topicId, question, options.ToList());
        return $"Poll created: {poll.Question}";
    },
    "CreatePoll");
```

<!-- speaker:
Open src/ConferenceAssistant.Agents/Tools/PollTools.cs and show the CreatePoll tool.
Say: "AIFunctionFactory.Create takes a lambda or method, reads the Description
attributes, and generates the JSON schema the model needs to call it.
Your .NET code becomes an AI tool. That's it."
-->

---

## Why It Matters

- **One interface** → any provider
- **Zero coupling** → swap Azure OpenAI for Ollama in one line
- **Middleware** → cross-cutting concerns, not boilerplate
- **Tool bridge** → AI reasoning meets your code

<!-- speaker:
Summarize the segment.
Say: "Before M.E.AI, every provider had its own SDK. Switch providers, rewrite
your integration. M.E.AI eliminates that entirely."

Launch the suggested poll about AI experience level.
Say: "Let's see where everyone is. Vote now — what's your experience with AI in .NET?"
Wait for poll results to come in. Comment on the distribution.
-->

---

<!-- topic: knowledge -->
<!-- layout: centered -->

# Knowledge Engineering

## DataIngestion + VectorData

<!-- speaker:
Transition: "Great — now let's talk about giving AI something to be smart about.
An LLM without knowledge is just pattern matching."
15 minutes for this segment.
-->

---

## The 5-Stage Pipeline

1. **Reader** — parse source documents (Markdown, PDF, etc.)
2. **DocumentProcessors** — pre-process the full document
3. **Chunker** — split into meaningful pieces
4. **ChunkProcessors** — enrich each chunk (summaries, keywords)
5. **Writer** — persist to a vector store

<!-- speaker:
Say: "Think of it as an assembly line for knowledge. Raw content goes in,
searchable semantic chunks come out."
Draw attention to stages 3 and 4 — that's where the magic happens.
"Chunking strategy is everything. Too big and you lose precision.
Too small and you lose context."
-->

---

## The Pipeline in Code

```csharp
IngestionDocumentReader reader = new MarkdownReader();
IngestionChunker<string> chunker = new HeaderChunker(
    new(tokenizer) { MaxTokensPerChunk = 500 });

using var writer = new VectorStoreWriter<string>(
    searchService.VectorStore, dimensionCount: 1536);

using IngestionPipeline<string> pipeline = new(
    reader, chunker, writer) {
    ChunkProcessors = { summaryEnricher, keywordEnricher }
};

await foreach (var result in pipeline.ProcessAsync(dir, file))
    count++;
```

<!-- speaker:
Open src/ConferenceAssistant.Ingestion/Services/IngestionService.cs.
Walk through: "MarkdownReader parses the file. HeaderChunker splits on headings
with a 500-token limit. SummaryEnricher and KeywordEnricher use the LLM to add
metadata. VectorStoreWriter embeds and stores."
Emphasize: Each component is pluggable.
-->

---

## VectorData — Semantic Search

```csharp
var store = new InMemoryVectorStore();
var collection = store.GetCollection<string, ConferenceRecord>(
    "conference-knowledge");

var embedding = await embeddingGenerator
    .GenerateVectorAsync(query);

await foreach (var result in collection
    .SearchAsync(embedding, topK: 5))
{
    records.Add(result.Record);
}
```

<!-- speaker:
Open src/ConferenceAssistant.Ingestion/Services/SemanticSearchService.cs.
Say: "InMemoryVectorStore — zero infrastructure. No database to set up.
Generate an embedding from the query, call SearchAsync, get ranked results.
In production, swap to Azure AI Search or Qdrant — same interface."
-->

---

## The Living Knowledge Base

- Session outline ingested at **startup**
- Poll responses ingested after **each vote**
- Audience questions ingested **in real-time**
- By the end, agents know **everything we discussed**

<!-- speaker:
Say: "This is what makes this session different. The knowledge base is growing
RIGHT NOW. Your poll responses from the last segment? Already ingested.
The agents are already smarter than they were 10 minutes ago."

Launch the suggested poll about RAG challenges.
Say: "Speaking of knowledge — what's the hardest part of RAG for you?"
Wait for results.
-->

---

<!-- topic: agents -->
<!-- layout: centered -->

# Agentic AI

## The Microsoft Agent Framework

<!-- speaker:
Transition: "We have abstractions. We have knowledge. Now let's give AI the
ability to take action."
15 minutes for this segment.
Ask: "Who here has built an AI agent — not just a chatbot, but something that
can call functions, use tools, and make decisions?"
-->

---

## ChatClientAgent

- Built on **IChatClient** — same abstraction
- **System instructions** define persona and behavior
- **Tools** via AIFunctionFactory integration
- **Structured output** for type-safe responses

<!-- speaker:
Say: "ChatClientAgent is the core building block. It wraps IChatClient and adds
agent capabilities — instructions, tools, and structured output.
It's not a new SDK. It's built on the same M.E.AI abstractions we just covered."
-->

---

## Our Three Agents

- 🏗️ **Survey Architect** — crafts contextual poll questions
- 📊 **Response Analyst** — interprets results, spots trends
- 📚 **Knowledge Curator** — manages the knowledge base

<!-- speaker:
Say: "This app has three specialized agents. The Survey Architect reads the
knowledge base and generates polls relevant to what we're discussing.
The Response Analyst interprets your votes and generates insights.
The Knowledge Curator enriches everything and retrieves context."
Open src/ConferenceAssistant.Agents/Definitions/AgentDefinitions.cs
and show the instruction strings briefly.
-->

---

## Agents in Code

```csharp
ChatClientAgent pollAnalyst = new(
    chatClient,
    name: "PollAnalyst",
    description: "Analyzes poll results and trends",
    instructions: """
        You are a poll analyst.
        Use GetAllPollResults to retrieve data.
        Identify patterns across segments...""",
    tools: aiTools);
```

<!-- speaker:
Open src/ConferenceAssistant.Agents/Workflows/SessionSummaryWorkflow.cs.
Say: "Creating an agent is straightforward — give it a chat client, a name,
instructions that define its behavior, and tools it can call.
The instructions ARE the agent's personality and capabilities."
-->

---

## Multi-Agent Workflows

```csharp
var workflow = AgentWorkflowBuilder.BuildConcurrent(
    [pollAnalyst, questionAnalyst, insightAnalyst],
    MergeAgentOutputs);

var run = await InProcessExecution.Default.RunAsync(
    workflow,
    "Analyze the session and provide findings.");
```

- **Sequential** — agents in order, building on each other
- **Concurrent** — fan-out, then merge results
- **Handoff** — pass control based on conditions

<!-- speaker:
Say: "AgentWorkflowBuilder lets you compose agents. BuildConcurrent runs them
all in parallel and merges the results. Sequential chains them.
Handoff lets one agent decide which agent should go next."
Demo: Show the SessionSummaryWorkflow file.
-->

---

## The Snowball Effect

- Segment 1 polls → **generic** (no context yet)
- Segment 2 polls → **informed** (outline + first responses)
- Segment 3 polls → **targeted** (full context + trends)
- Each segment makes agents **smarter**

<!-- speaker:
Say: "This is the power of a growing knowledge base. The agents right now have
context from the outline, your poll responses, and accumulated insights.
Notice how the polls are getting more specific? That's not scripted.
That's the agents using everything they've learned."

Launch the suggested poll about agent patterns.
Say: "Which agent pattern would you reach for first in your own projects?"
-->

---

<!-- topic: mcp -->
<!-- layout: centered -->

# Interoperability

## Model Context Protocol

<!-- speaker:
Transition: "We've built agents with tools and knowledge. But what if other
AI applications could use our capabilities too? That's MCP."
10 minutes for this segment.
-->

---

## MCP Server: Our Exposed Tools

```csharp
[McpServerToolType]
public class ConferenceTools
{
    [McpServerTool(Name = "search_session_knowledge")]
    [Description("Search the session knowledge base")]
    public static async Task<string> SearchSessionKnowledge(
        ISemanticSearchService searchService,
        [Description("The search query")] string query,
        [Description("Max results")] int maxResults = 5)
    {
        var results = await searchService.SearchAsync(
            query, maxResults);
        // ...
    }
}
```

<!-- speaker:
Open src/ConferenceAssistant.Mcp/Tools/ConferenceTools.cs.
Say: "Decorate a static method with McpServerTool. Add Description attributes.
DI parameters are auto-resolved. That's it — your app just became an MCP server."
List the tools: get_session_status, get_active_poll, get_poll_results,
search_session_knowledge, get_audience_questions, generate_session_summary.
-->

---

## Wiring Up the MCP Server

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
    .WithToolsFromAssembly(typeof(ConferenceTools).Assembly)
    .WithHttpTransport();

app.MapMcp("/mcp");
```

<!-- speaker:
Show Program.cs where MCP is registered.
Say: "AddMcpServer, discover tools from the assembly, enable HTTP transport,
map the endpoint. Any MCP-compatible client can now connect —
GitHub Copilot, VS Code, Claude, custom apps."
-->

---

## The Ecosystem Effect

- **Your app** → MCP server → tools for any AI client
- **External servers** → MCP client → knowledge for your agents
- **No custom integrations** — just a protocol
- **Composable AI** across the entire ecosystem

<!-- speaker:
Say: "MCP creates an ecosystem. Your AI app's capabilities become building blocks
that any other AI app can use. And your app can consume tools from any MCP server.
It's like REST APIs but for AI tool calling."

Launch the suggested poll about MCP excitement.
Say: "What excites you most about MCP? Vote now."
-->

---

<!-- topic: closer -->
<!-- layout: centered -->

# The Closer

## Full Stack in One Command

<!-- speaker:
Transition: "Everything we've discussed — abstractions, knowledge, agents,
interoperability — comes together right now."
5 minutes for this segment. Build the energy.
Say: "Watch the big screen. One command. Every technology."
-->

---

## The Cascade

1. **Copilot** sends request → MCP server
2. **MCP** discovers `generate_session_summary` tool
3. **Agent Framework** orchestrates the workflow
4. **VectorData** searches all accumulated knowledge
5. **DataIngestion** — all that knowledge was built by pipelines
6. **M.E.AI** — every call flows through the abstractions

<!-- speaker:
Say: "We're going to connect GitHub Copilot to our MCP server and ask it
to summarize this session. Watch what happens."
Build anticipation. This is the climax of the talk.
Demo: Connect Copilot to the /mcp endpoint and type:
"Summarize this session including poll results and audience questions."
Watch the cascade visualization on /display.
-->

---

## One Command. Five Technologies.

```csharp
builder.Services.AddMcpServer(options => {
    options.ServerInfo = new() {
        Name = "ConferencePulse", Version = "1.0.0"
    };
})
.WithToolsFromAssembly(typeof(ConferenceTools).Assembly)
.WithHttpTransport();

// That's it. Copilot does the rest.
// MCP → Agents → VectorData → Ingestion → M.E.AI
```

<!-- speaker:
Say: "This is all the code it took to expose our entire application to Copilot.
Seven lines. The agent framework, vector search, ingestion pipelines,
and M.E.AI abstractions — they're all already wired up.
MCP just opens the door."
Let the cascade visualization finish on screen.
-->

---

## What You Saw Today

- **M.E.AI** — provider-agnostic AI abstractions
- **DataIngestion** — documents → searchable knowledge
- **VectorData** — semantic search, any store
- **Agent Framework** — agents with tools & workflows
- **MCP** — universal AI interoperability

<!-- speaker:
Recap quickly. Point to each item.
Say: "Five technologies. One application. Built live in 60 minutes.
And the best part? Every one of these is open source and shipping now."
-->

---

<!-- layout: centered -->

## Go Build Something

- 📦 **M.E.AI** — github.com/dotnet/extensions
- 🤖 **Agent Framework** — github.com/microsoft/agent-framework
- 🔌 **MCP for .NET** — github.com/modelcontextprotocol/csharp-sdk
- 🔍 **VectorData** — learn.microsoft.com/dotnet/ai
- 💻 **This app** — scan the QR code

<!-- speaker:
Say: "All the links are on screen. The source code for this entire application
is available — scan the QR code."
Pause for people to take photos of the screen.
-->

---

<!-- layout: centered -->

# Thank You

### The presentation that presented itself

<!-- speaker:
Final slide. Say: "Thank you all for being part of this experiment.
You didn't just watch a presentation — you shaped it. Your votes,
your questions, your participation made the agents smarter and the
content more relevant. That's the power of the Microsoft AI stack for .NET."

If time permits, take 2-3 live questions.
Launch the final poll: "Rate this session format."
-->
