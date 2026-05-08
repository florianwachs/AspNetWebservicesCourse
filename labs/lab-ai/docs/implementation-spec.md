# Conference Pulse — Implementation Specification

> **Target audience**: Junior developer. Every component has exact file paths, class signatures, code patterns, and acceptance criteria. Follow components in order; each builds on the previous.

---

## Component 1: scaffold-solution

### What to create

**`Directory.Build.props`** (repo root):
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**`Directory.Packages.props`** (repo root):
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- M.E.AI -->
    <PackageVersion Include="Microsoft.Extensions.AI" Version="9.5.0" />
    <PackageVersion Include="Microsoft.Extensions.AI.Abstractions" Version="9.5.0" />
    <PackageVersion Include="Microsoft.Extensions.AI.OpenAI" Version="9.5.0" />

    <!-- DataIngestion -->
    <PackageVersion Include="Microsoft.Extensions.DataIngestion" Version="10.4.0-preview" />
    <PackageVersion Include="Microsoft.Extensions.DataIngestion.Markdig" Version="10.4.0-preview" />

    <!-- VectorData -->
    <PackageVersion Include="Microsoft.Extensions.VectorData.Abstractions" Version="9.5.0" />
    <PackageVersion Include="Microsoft.SemanticKernel.Connectors.InMemory" Version="1.47.0" />

    <!-- Agent Framework -->
    <PackageVersion Include="Microsoft.Agents.AI" Version="0.3.0-preview" />
    <PackageVersion Include="Microsoft.Agents.AI.OpenAI" Version="0.3.0-preview" />
    <PackageVersion Include="Microsoft.Agents.AI.Workflows" Version="0.3.0-preview" />

    <!-- MCP -->
    <PackageVersion Include="ModelContextProtocol" Version="0.2.0-preview" />
    <PackageVersion Include="ModelContextProtocol.AspNetCore" Version="0.2.0-preview" />

    <!-- Copilot SDK -->
    <PackageVersion Include="GitHub.Copilot.SDK" Version="0.1.29" />

    <!-- Aspire -->
    <PackageVersion Include="Aspire.Hosting.AppHost" Version="9.2.0" />
    <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.5.0" />
    <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.2.0" />
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.11.2" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.11.2" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.11.2" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.11.2" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.11.2" />

    <!-- Testing -->
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />
    <PackageVersion Include="xunit" Version="2.9.3" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.0.2" />
    <PackageVersion Include="NSubstitute" Version="5.3.0" />
  </ItemGroup>
</Project>
```

> **Note**: Package versions are approximate. Run `dotnet add package <name>` to get latest versions and update Directory.Packages.props accordingly.

**`.gitignore`**: Use `dotnet new gitignore`.

**Solution file**: `dotnet new sln -n ConferenceAssistant`

**Projects** (create in order, then add to sln):
```bash
dotnet new classlib -n ConferenceAssistant.Core -o src/ConferenceAssistant.Core
dotnet new classlib -n ConferenceAssistant.Ingestion -o src/ConferenceAssistant.Ingestion
dotnet new classlib -n ConferenceAssistant.Agents -o src/ConferenceAssistant.Agents
dotnet new classlib -n ConferenceAssistant.Mcp -o src/ConferenceAssistant.Mcp
dotnet new blazor -n ConferenceAssistant.Web -o src/ConferenceAssistant.Web --interactivity Server
dotnet new console -n ConferenceAssistant.CopilotDemo -o src/ConferenceAssistant.CopilotDemo
dotnet new aspire-apphost -n ConferenceAssistant.AppHost -o src/ConferenceAssistant.AppHost
dotnet new aspire-servicedefaults -n ConferenceAssistant.ServiceDefaults -o src/ConferenceAssistant.ServiceDefaults
dotnet new xunit -n ConferenceAssistant.Agents.Tests -o tests/ConferenceAssistant.Agents.Tests
dotnet new xunit -n ConferenceAssistant.Ingestion.Tests -o tests/ConferenceAssistant.Ingestion.Tests
dotnet new xunit -n ConferenceAssistant.Mcp.Tests -o tests/ConferenceAssistant.Mcp.Tests

# Add all to solution
dotnet sln add src/ConferenceAssistant.Core
dotnet sln add src/ConferenceAssistant.Ingestion
dotnet sln add src/ConferenceAssistant.Agents
dotnet sln add src/ConferenceAssistant.Mcp
dotnet sln add src/ConferenceAssistant.Web
dotnet sln add src/ConferenceAssistant.CopilotDemo
dotnet sln add src/ConferenceAssistant.AppHost
dotnet sln add src/ConferenceAssistant.ServiceDefaults
dotnet sln add tests/ConferenceAssistant.Agents.Tests
dotnet sln add tests/ConferenceAssistant.Ingestion.Tests
dotnet sln add tests/ConferenceAssistant.Mcp.Tests
```

**Project references** (add after creating all projects):
```bash
# Ingestion references Core
dotnet add src/ConferenceAssistant.Ingestion reference src/ConferenceAssistant.Core

# Agents references Core + Ingestion
dotnet add src/ConferenceAssistant.Agents reference src/ConferenceAssistant.Core
dotnet add src/ConferenceAssistant.Agents reference src/ConferenceAssistant.Ingestion

# Mcp references Core + Ingestion + Agents
dotnet add src/ConferenceAssistant.Mcp reference src/ConferenceAssistant.Core
dotnet add src/ConferenceAssistant.Mcp reference src/ConferenceAssistant.Ingestion
dotnet add src/ConferenceAssistant.Mcp reference src/ConferenceAssistant.Agents

# Web references all library projects + ServiceDefaults
dotnet add src/ConferenceAssistant.Web reference src/ConferenceAssistant.Core
dotnet add src/ConferenceAssistant.Web reference src/ConferenceAssistant.Ingestion
dotnet add src/ConferenceAssistant.Web reference src/ConferenceAssistant.Agents
dotnet add src/ConferenceAssistant.Web reference src/ConferenceAssistant.Mcp
dotnet add src/ConferenceAssistant.Web reference src/ConferenceAssistant.ServiceDefaults

# Tests reference their targets
dotnet add tests/ConferenceAssistant.Agents.Tests reference src/ConferenceAssistant.Agents
dotnet add tests/ConferenceAssistant.Ingestion.Tests reference src/ConferenceAssistant.Ingestion
dotnet add tests/ConferenceAssistant.Mcp.Tests reference src/ConferenceAssistant.Mcp
```

**Done when**: `dotnet build` succeeds with zero errors and zero warnings.

---

## Component 2: core-models

### Files to create

**`src/ConferenceAssistant.Core/Models/Poll.cs`**:
```csharp
namespace ConferenceAssistant.Core.Models;

public class Poll
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TopicId { get; set; } = "";
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = [];
    public PollStatus Status { get; set; } = PollStatus.Draft;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }
}

public enum PollStatus
{
    Draft,
    Active,
    Closed
}
```

**`src/ConferenceAssistant.Core/Models/PollResponse.cs`**:
```csharp
namespace ConferenceAssistant.Core.Models;

public class PollResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PollId { get; set; } = "";
    public string SelectedOption { get; set; } = "";
    public string? AttendeeId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
```

**`src/ConferenceAssistant.Core/Models/SessionTopic.cs`**:
```csharp
namespace ConferenceAssistant.Core.Models;

public class SessionTopic
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Order { get; set; }
    public TopicStatus Status { get; set; } = TopicStatus.Upcoming;
    public List<string> TalkingPoints { get; set; } = [];
    public List<PollPrompt> PollPrompts { get; set; } = [];
}

public enum TopicStatus
{
    Upcoming,
    Active,
    Completed
}

public class PollPrompt
{
    public string Context { get; set; } = "";
    public string Hint { get; set; } = "";
}
```

**`src/ConferenceAssistant.Core/Models/Insight.cs`**:
```csharp
namespace ConferenceAssistant.Core.Models;

public class Insight
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PollId { get; set; } = "";
    public string Content { get; set; } = "";
    public InsightType Type { get; set; }
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
}

public enum InsightType
{
    PollAnalysis,
    AudienceTrend,
    KnowledgeGap,
    Summary
}
```

**`src/ConferenceAssistant.Core/Models/AudienceQuestion.cs`**:
```csharp
namespace ConferenceAssistant.Core.Models;

public class AudienceQuestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = "";
    public string? Answer { get; set; }
    public string? AttendeeId { get; set; }
    public int Upvotes { get; set; }
    public DateTimeOffset AskedAt { get; set; } = DateTimeOffset.UtcNow;
}
```

**Done when**: `dotnet build src/ConferenceAssistant.Core` succeeds. All models have parameterless constructors and default values.

---

## Component 3: core-services

### Files to create

**`src/ConferenceAssistant.Core/Services/InMemoryStore.cs`**:
```csharp
using System.Collections.Concurrent;

namespace ConferenceAssistant.Core.Services;

public class InMemoryStore<T> where T : class
{
    private readonly ConcurrentDictionary<string, T> _items = new();

    public void Add(string key, T item) => _items[key] = item;
    public T? Get(string key) => _items.TryGetValue(key, out var item) ? item : null;
    public IReadOnlyList<T> GetAll() => _items.Values.ToList();
    public bool Remove(string key) => _items.TryRemove(key, out _);
    public IReadOnlyList<T> Query(Func<T, bool> predicate) => _items.Values.Where(predicate).ToList();
}
```

**`src/ConferenceAssistant.Core/Services/PollService.cs`**:
```csharp
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class PollService
{
    private readonly InMemoryStore<Poll> _polls = new();
    private readonly InMemoryStore<PollResponse> _responses = new();

    public Poll CreatePoll(string topicId, string question, List<string> options)
    {
        var poll = new Poll
        {
            TopicId = topicId,
            Question = question,
            Options = options
        };
        _polls.Add(poll.Id, poll);
        return poll;
    }

    public Poll? GetPoll(string pollId) => _polls.Get(pollId);

    public IReadOnlyList<Poll> GetActivePolls()
        => _polls.Query(p => p.Status == PollStatus.Active);

    public IReadOnlyList<Poll> GetAllPolls() => _polls.GetAll();

    public void LaunchPoll(string pollId)
    {
        var poll = _polls.Get(pollId)
            ?? throw new InvalidOperationException($"Poll {pollId} not found");
        poll.Status = PollStatus.Active;
    }

    public void ClosePoll(string pollId)
    {
        var poll = _polls.Get(pollId)
            ?? throw new InvalidOperationException($"Poll {pollId} not found");
        poll.Status = PollStatus.Closed;
        poll.ClosedAt = DateTimeOffset.UtcNow;
    }

    public PollResponse SubmitResponse(string pollId, string selectedOption, string? attendeeId = null)
    {
        var poll = _polls.Get(pollId)
            ?? throw new InvalidOperationException($"Poll {pollId} not found");
        if (poll.Status != PollStatus.Active)
            throw new InvalidOperationException($"Poll {pollId} is not active");

        var response = new PollResponse
        {
            PollId = pollId,
            SelectedOption = selectedOption,
            AttendeeId = attendeeId
        };
        _responses.Add(response.Id, response);
        return response;
    }

    public IReadOnlyList<PollResponse> GetResponses(string pollId)
        => _responses.Query(r => r.PollId == pollId);

    public Dictionary<string, int> GetResultTally(string pollId)
    {
        var poll = _polls.Get(pollId);
        if (poll is null) return new();

        var responses = GetResponses(pollId);
        return poll.Options.ToDictionary(
            option => option,
            option => responses.Count(r => r.SelectedOption == option)
        );
    }
}
```

**`src/ConferenceAssistant.Core/Services/SessionService.cs`**:
```csharp
using System.Text.Json;
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class SessionService
{
    private readonly InMemoryStore<SessionTopic> _topics = new();
    private readonly InMemoryStore<Insight> _insights = new();
    private readonly InMemoryStore<AudienceQuestion> _questions = new();

    private string _currentTopicId = "";
    public string SessionCode { get; set; } = "AICONF";

    public async Task LoadTopicsAsync(string jsonPath)
    {
        var json = await File.ReadAllTextAsync(jsonPath);
        var topics = JsonSerializer.Deserialize<List<SessionTopic>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Failed to deserialize topics");

        foreach (var topic in topics.OrderBy(t => t.Order))
        {
            _topics.Add(topic.Id, topic);
        }

        if (topics.Count > 0)
        {
            _currentTopicId = topics.OrderBy(t => t.Order).First().Id;
            var first = _topics.Get(_currentTopicId);
            if (first is not null) first.Status = TopicStatus.Active;
        }
    }

    public SessionTopic? GetCurrentTopic()
        => string.IsNullOrEmpty(_currentTopicId) ? null : _topics.Get(_currentTopicId);

    public IReadOnlyList<SessionTopic> GetAllTopics()
        => _topics.GetAll().OrderBy(t => t.Order).ToList();

    public SessionTopic? AdvanceToNextTopic()
    {
        var topics = GetAllTopics();
        var currentIndex = topics.ToList().FindIndex(t => t.Id == _currentTopicId);

        if (currentIndex >= 0 && currentIndex < topics.Count - 1)
        {
            var current = _topics.Get(_currentTopicId);
            if (current is not null) current.Status = TopicStatus.Completed;

            _currentTopicId = topics[currentIndex + 1].Id;
            var next = _topics.Get(_currentTopicId);
            if (next is not null) next.Status = TopicStatus.Active;
            return next;
        }
        return null;
    }

    public void AddInsight(Insight insight) => _insights.Add(insight.Id, insight);

    public IReadOnlyList<Insight> GetInsights()
        => _insights.GetAll().OrderByDescending(i => i.GeneratedAt).ToList();

    public IReadOnlyList<Insight> GetInsightsForPoll(string pollId)
        => _insights.Query(i => i.PollId == pollId);

    public AudienceQuestion AddQuestion(string text, string? attendeeId = null)
    {
        var question = new AudienceQuestion { Text = text, AttendeeId = attendeeId };
        _questions.Add(question.Id, question);
        return question;
    }

    public void AnswerQuestion(string questionId, string answer)
    {
        var q = _questions.Get(questionId);
        if (q is not null) q.Answer = answer;
    }

    public void UpvoteQuestion(string questionId)
    {
        var q = _questions.Get(questionId);
        if (q is not null) Interlocked.Increment(ref q.Upvotes);
    }

    public IReadOnlyList<AudienceQuestion> GetQuestions()
        => _questions.GetAll().OrderByDescending(q => q.Upvotes).ToList();
}
```

**Done when**: `dotnet build src/ConferenceAssistant.Core` succeeds. PollService can create, launch, close polls, submit responses, and tally results. SessionService can load topics from JSON and advance through them.

---

## Component 4: ingestion-record-model

### File to create

**`src/ConferenceAssistant.Ingestion/Models/ConferenceRecord.cs`**:
```csharp
using Microsoft.Extensions.VectorData;

namespace ConferenceAssistant.Ingestion.Models;

public class ConferenceRecord
{
    [VectorStoreKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [VectorStoreData]
    public string Content { get; set; } = "";

    [VectorStoreData]
    public string Source { get; set; } = "";  // "outline", "response", "question", "mcp-doc"

    [VectorStoreData]
    public string TopicId { get; set; } = "";

    [VectorStoreData]
    public string? Summary { get; set; }

    [VectorStoreData]
    public List<string> Keywords { get; set; } = [];

    [VectorStoreData]
    public string? Sentiment { get; set; }

    [VectorStoreData]
    public DateTimeOffset IngestedAt { get; set; } = DateTimeOffset.UtcNow;

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
```

**Done when**: `dotnet build src/ConferenceAssistant.Ingestion` succeeds.

---

## Component 5: ingestion-outline-pipeline

### File to create

**`src/ConferenceAssistant.Ingestion/Pipelines/OutlineIngestionPipeline.cs`**:

This uses `Microsoft.Extensions.DataIngestion` to ingest the session outline markdown:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.VectorData;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Pipelines;

public class OutlineIngestionPipeline(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStoreRecordCollection<string, ConferenceRecord> vectorStore)
{
    public async Task IngestAsync(string markdownPath, CancellationToken ct = default)
    {
        var markdown = await File.ReadAllTextAsync(markdownPath, ct);

        // Build the ingestion pipeline
        var pipeline = new IngestionPipelineBuilder<ConferenceRecord>()
            .WithReader(new MarkdigReader())
            .WithChunker(new HeaderChunker())
            .AddChunkProcessor(new SummaryEnricher(chatClient))
            .AddChunkProcessor(new KeywordEnricher(chatClient))
            .WithWriter(new VectorStoreWriter<ConferenceRecord>(vectorStore, embeddingGenerator))
            .Build();

        await pipeline.RunAsync(markdown, new IngestionContext { Source = "outline" }, ct);
    }
}
```

> **Implementation note**: The exact API of `IngestionPipelineBuilder<T>`, `MarkdigReader`, `HeaderChunker`, `SummaryEnricher`, `KeywordEnricher`, and `VectorStoreWriter` depends on the `Microsoft.Extensions.DataIngestion` package version. Check the package's API surface and adapt the builder pattern accordingly. The enrichers (`SummaryEnricher`, `KeywordEnricher`) use `IChatClient` to generate summaries/keywords for each chunk. If the package doesn't include these enrichers, implement them as custom `IChunkProcessor<ConferenceRecord>` implementations that call `IChatClient.GetResponseAsync()`.

**Custom enricher pattern (if needed)**:
```csharp
public class SummaryEnricher(IChatClient chatClient) : IChunkProcessor<ConferenceRecord>
{
    public async Task<ConferenceRecord> ProcessAsync(ConferenceRecord record, CancellationToken ct = default)
    {
        var response = await chatClient.GetResponseAsync(
            $"Summarize this text in one sentence:\n\n{record.Content}", ct);
        record.Summary = response.Text;
        return record;
    }
}

public class KeywordEnricher(IChatClient chatClient) : IChunkProcessor<ConferenceRecord>
{
    public async Task<ConferenceRecord> ProcessAsync(ConferenceRecord record, CancellationToken ct = default)
    {
        var response = await chatClient.GetResponseAsync(
            $"Extract 3-5 keywords from this text. Return them comma-separated:\n\n{record.Content}", ct);
        record.Keywords = response.Text?.Split(',', StringSplitOptions.TrimEntries).ToList() ?? [];
        return record;
    }
}
```

**Done when**: Pipeline can read a markdown file, chunk it by headings, enrich chunks with summaries and keywords, and store them with embeddings in the vector store.

---

## Component 6: ingestion-response-pipeline

### File to create

**`src/ConferenceAssistant.Ingestion/Pipelines/ResponseIngestionPipeline.cs`**:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Pipelines;

public class ResponseIngestionPipeline(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStoreRecordCollection<string, ConferenceRecord> vectorStore)
{
    public async Task IngestResponsesAsync(
        string pollId,
        string question,
        Dictionary<string, int> tally,
        CancellationToken ct = default)
    {
        // Format responses as readable text
        var totalVotes = tally.Values.Sum();
        var resultText = $"Poll: {question}\nTotal votes: {totalVotes}\n" +
            string.Join("\n", tally.Select(kv =>
                $"- {kv.Key}: {kv.Value} votes ({(totalVotes > 0 ? kv.Value * 100 / totalVotes : 0)}%)"));

        // Detect sentiment
        var sentimentResponse = await chatClient.GetResponseAsync(
            $"Classify the overall audience sentiment from these poll results as one word (positive, negative, neutral, mixed, curious, confused):\n\n{resultText}", ct);

        // Extract keywords
        var keywordResponse = await chatClient.GetResponseAsync(
            $"Extract 3-5 keywords from these poll results. Comma-separated:\n\n{resultText}", ct);

        var record = new ConferenceRecord
        {
            Content = resultText,
            Source = "response",
            TopicId = pollId,
            Sentiment = sentimentResponse.Text?.Trim(),
            Keywords = keywordResponse.Text?.Split(',', StringSplitOptions.TrimEntries).ToList() ?? []
        };

        // Generate embedding and store
        var embedding = await embeddingGenerator.GenerateAsync(record.Content, ct);
        record.Embedding = embedding.Vector;
        await vectorStore.UpsertAsync(record, ct);
    }
}
```

**Done when**: Can take poll tally data, format it, enrich with sentiment and keywords, and store in vector store.

---

## Component 7: ingestion-mcp-pipeline

### File to create

**`src/ConferenceAssistant.Ingestion/Pipelines/McpContentIngestionPipeline.cs`**:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Pipelines;

public class McpContentIngestionPipeline(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStoreRecordCollection<string, ConferenceRecord> vectorStore)
{
    public async Task IngestAsync(string content, string sourceUrl, CancellationToken ct = default)
    {
        // Chunk by paragraphs (simple chunking for MCP content)
        var chunks = ChunkByParagraphs(content, maxTokens: 500);

        foreach (var chunk in chunks)
        {
            var summaryResponse = await chatClient.GetResponseAsync(
                $"Summarize in one sentence:\n\n{chunk}", ct);

            var record = new ConferenceRecord
            {
                Content = chunk,
                Source = "mcp-doc",
                TopicId = sourceUrl,
                Summary = summaryResponse.Text?.Trim()
            };

            var embedding = await embeddingGenerator.GenerateAsync(record.Content, ct);
            record.Embedding = embedding.Vector;
            await vectorStore.UpsertAsync(record, ct);
        }
    }

    private static List<string> ChunkByParagraphs(string content, int maxTokens)
    {
        var paragraphs = content.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var current = "";

        foreach (var paragraph in paragraphs)
        {
            if ((current.Length + paragraph.Length) / 4 > maxTokens && current.Length > 0)
            {
                chunks.Add(current.Trim());
                current = "";
            }
            current += paragraph + "\n\n";
        }

        if (current.Trim().Length > 0)
            chunks.Add(current.Trim());

        return chunks;
    }
}
```

**Done when**: Can take MCP-fetched content, chunk it, summarize, and store in vector store.

---

## Component 8: semantic-search-service

### File to create

**`src/ConferenceAssistant.Ingestion/SemanticSearchService.cs`**:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion;

public class SemanticSearchService(
    IVectorStoreRecordCollection<string, ConferenceRecord> vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    public async Task<IReadOnlyList<ConferenceRecord>> SearchAsync(
        string query,
        int topK = 5,
        CancellationToken ct = default)
    {
        var queryEmbedding = await embeddingGenerator.GenerateAsync(query, ct);

        var results = await vectorStore.SearchAsync(
            queryEmbedding.Vector,
            new VectorSearchOptions { Top = topK },
            ct);

        var records = new List<ConferenceRecord>();
        await foreach (var result in results)
        {
            records.Add(result.Record);
        }
        return records;
    }

    public async Task<IReadOnlyList<ConferenceRecord>> SearchBySourceAsync(
        string query,
        string source,
        int topK = 5,
        CancellationToken ct = default)
    {
        var all = await SearchAsync(query, topK * 2, ct);
        return all.Where(r => r.Source == source).Take(topK).ToList();
    }
}
```

**`src/ConferenceAssistant.Ingestion/DependencyInjection.cs`**:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using ConferenceAssistant.Ingestion.Models;
using ConferenceAssistant.Ingestion.Pipelines;

namespace ConferenceAssistant.Ingestion;

public static class IngestionServiceCollectionExtensions
{
    public static IServiceCollection AddIngestionServices(this IServiceCollection services)
    {
        // Vector store
        services.AddSingleton<InMemoryVectorStore>();
        services.AddSingleton<IVectorStoreRecordCollection<string, ConferenceRecord>>(sp =>
        {
            var store = sp.GetRequiredService<InMemoryVectorStore>();
            return store.GetCollection<string, ConferenceRecord>("conference-knowledge");
        });

        // Pipelines
        services.AddSingleton<OutlineIngestionPipeline>();
        services.AddSingleton<ResponseIngestionPipeline>();
        services.AddSingleton<McpContentIngestionPipeline>();

        // Search
        services.AddSingleton<SemanticSearchService>();

        return services;
    }
}
```

**Done when**: `SemanticSearchService.SearchAsync("embeddings")` returns relevant records from the vector store. DI extension registers all ingestion services.

---

## Component 9: agent-tools

### Files to create

**`src/ConferenceAssistant.Agents/Tools/PollTools.cs`**:
```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Core.Services;

namespace ConferenceAssistant.Agents.Tools;

public static class PollTools
{
    public static AIFunction CreateCreatePollTool(PollService pollService)
        => AIFunctionFactory.Create(
            (string topicId, string question, string[] options) =>
            {
                var poll = pollService.CreatePoll(topicId, question, options.ToList());
                return $"Poll created with ID: {poll.Id}. Question: {poll.Question}. Options: {string.Join(", ", poll.Options)}";
            },
            "create_poll",
            "Create a new poll for a session topic. Returns the poll ID.");

    public static AIFunction CreateGetPollResultsTool(PollService pollService)
        => AIFunctionFactory.Create(
            (string pollId) =>
            {
                var poll = pollService.GetPoll(pollId);
                if (poll is null) return "Poll not found.";
                var tally = pollService.GetResultTally(pollId);
                var total = tally.Values.Sum();
                var results = string.Join("\n", tally.Select(kv =>
                    $"- {kv.Key}: {kv.Value} votes ({(total > 0 ? kv.Value * 100 / total : 0)}%)"));
                return $"Poll: {poll.Question}\nTotal votes: {total}\n{results}";
            },
            "get_poll_results",
            "Get the current results of a poll by ID.");

    public static AIFunction CreateClosePollTool(PollService pollService)
        => AIFunctionFactory.Create(
            (string pollId) =>
            {
                pollService.ClosePoll(pollId);
                return $"Poll {pollId} closed.";
            },
            "close_poll",
            "Close an active poll, stopping further votes.");
}
```

**`src/ConferenceAssistant.Agents/Tools/KnowledgeTools.cs`**:
```csharp
using Microsoft.Extensions.AI;
using ConferenceAssistant.Ingestion;
using ConferenceAssistant.Ingestion.Pipelines;

namespace ConferenceAssistant.Agents.Tools;

public static class KnowledgeTools
{
    public static AIFunction CreateSearchKnowledgeTool(SemanticSearchService searchService)
        => AIFunctionFactory.Create(
            async (string query, int topK) =>
            {
                var results = await searchService.SearchAsync(query, topK > 0 ? topK : 5);
                if (results.Count == 0) return "No relevant knowledge found.";

                return string.Join("\n\n---\n\n", results.Select(r =>
                    $"[{r.Source}] {(r.Summary ?? r.Content[..Math.Min(200, r.Content.Length)])}"));
            },
            "search_knowledge",
            "Search the session knowledge base for relevant content. Returns top results.");

    public static AIFunction CreateIngestContentTool(McpContentIngestionPipeline mcpPipeline)
        => AIFunctionFactory.Create(
            async (string content, string sourceUrl) =>
            {
                await mcpPipeline.IngestAsync(content, sourceUrl);
                return $"Content from {sourceUrl} ingested into knowledge base.";
            },
            "ingest_content",
            "Ingest external content (e.g., from MCP) into the knowledge base.");
}
```

**`src/ConferenceAssistant.Agents/Tools/InsightTools.cs`**:
```csharp
using Microsoft.Extensions.AI;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Core.Services;

namespace ConferenceAssistant.Agents.Tools;

public static class InsightTools
{
    public static AIFunction CreateStoreInsightTool(SessionService sessionService)
        => AIFunctionFactory.Create(
            (string pollId, string content, string type) =>
            {
                var insightType = Enum.TryParse<InsightType>(type, true, out var t) ? t : InsightType.PollAnalysis;
                var insight = new Insight
                {
                    PollId = pollId,
                    Content = content,
                    Type = insightType
                };
                sessionService.AddInsight(insight);
                return $"Insight stored: {insight.Id}";
            },
            "store_insight",
            "Store an AI-generated insight for a poll or topic.");

    public static AIFunction CreateGetInsightsTool(SessionService sessionService)
        => AIFunctionFactory.Create(
            () =>
            {
                var insights = sessionService.GetInsights();
                if (insights.Count == 0) return "No insights generated yet.";
                return string.Join("\n\n", insights.Select(i =>
                    $"[{i.Type}] {i.Content}"));
            },
            "get_insights",
            "Get all AI-generated insights from the session.");

    public static AIFunction CreateGetSessionSummaryDataTool(
        SessionService sessionService, PollService pollService)
        => AIFunctionFactory.Create(
            () =>
            {
                var topics = sessionService.GetAllTopics();
                var insights = sessionService.GetInsights();
                var questions = sessionService.GetQuestions();
                var polls = pollService.GetAllPolls();

                var summary = $"Session: {sessionService.SessionCode}\n";
                summary += $"Topics covered: {topics.Count(t => t.Status == TopicStatus.Completed)}/{topics.Count}\n";
                summary += $"Polls run: {polls.Count}\n";
                summary += $"Questions asked: {questions.Count}\n";
                summary += $"Insights generated: {insights.Count}\n\n";

                summary += "## Poll Results\n";
                foreach (var poll in polls)
                {
                    var tally = pollService.GetResultTally(poll.Id);
                    summary += $"\n### {poll.Question}\n";
                    foreach (var (option, count) in tally)
                        summary += $"- {option}: {count}\n";
                }

                summary += "\n## Insights\n";
                foreach (var insight in insights)
                    summary += $"- [{insight.Type}] {insight.Content}\n";

                summary += "\n## Top Questions\n";
                foreach (var q in questions.Take(10))
                    summary += $"- {q.Text} (↑{q.Upvotes}){(q.Answer != null ? $" → {q.Answer}" : "")}\n";

                return summary;
            },
            "get_session_summary_data",
            "Get comprehensive session data including polls, insights, and questions.");
}
```

**Done when**: All three tool classes compile. Each `AIFunction` has a descriptive name and description.

---

## Component 10: survey-architect-agent

### File to create

**`src/ConferenceAssistant.Agents/SurveyArchitectAgent.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Tools;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;

namespace ConferenceAssistant.Agents;

public static class SurveyArchitectAgent
{
    public const string Name = "SurveyArchitect";

    public const string Instructions = """
        You are the Survey Architect. Your job is to generate engaging, insightful poll questions for a live conference session.

        When asked to generate a poll:
        1. First, use the search_knowledge tool to find context about the current topic and what the audience has already discussed.
        2. Consider the audience's demonstrated knowledge level from previous poll results.
        3. Generate a poll question that is:
           - Relevant to the current topic
           - Engaging and thought-provoking (not trivial yes/no)
           - Has 3-5 well-differentiated options
           - Designed to reveal interesting audience perspectives
        4. Use the create_poll tool to create the poll.

        Always search for context before generating. Your polls should get smarter as the session progresses.
        """;

    public static ChatClientAgent Create(
        IChatClient chatClient,
        PollService pollService,
        SemanticSearchService searchService)
    {
        return new ChatClientAgentBuilder()
            .WithChatClient(chatClient)
            .WithName(Name)
            .WithInstructions(Instructions)
            .WithTools([
                PollTools.CreateCreatePollTool(pollService),
                KnowledgeTools.CreateSearchKnowledgeTool(searchService)
            ])
            .Build();
    }
}
```

**Done when**: Agent can be created with all its tools. Agent uses `ChatClientAgentBuilder` from Agent Framework.

---

## Component 11: response-analyst-agent

### File to create

**`src/ConferenceAssistant.Agents/ResponseAnalystAgent.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Tools;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;

namespace ConferenceAssistant.Agents;

public static class ResponseAnalystAgent
{
    public const string Name = "ResponseAnalyst";

    public const string Instructions = """
        You are the Response Analyst. Your job is to interpret poll results and generate meaningful insights.

        When asked to analyze poll results:
        1. Use get_poll_results to get the raw data.
        2. Use search_knowledge to find context about the topic and audience.
        3. Analyze the results for:
           - What the majority choice reveals about the audience
           - What minority choices might indicate (misconceptions, advanced knowledge, etc.)
           - Patterns when compared to previous polls
           - Actionable insights for the speaker
        4. Use store_insight to save your analysis.

        Be specific and actionable. Don't just restate the numbers — interpret them.
        Example: "65% chose B (IChatClient), suggesting the audience values abstraction layers. The 20% who chose D (AIFunctionFactory) likely have hands-on agent experience."
        """;

    public static ChatClientAgent Create(
        IChatClient chatClient,
        PollService pollService,
        SessionService sessionService,
        SemanticSearchService searchService)
    {
        return new ChatClientAgentBuilder()
            .WithChatClient(chatClient)
            .WithName(Name)
            .WithInstructions(Instructions)
            .WithTools([
                PollTools.CreateGetPollResultsTool(pollService),
                InsightTools.CreateStoreInsightTool(sessionService),
                KnowledgeTools.CreateSearchKnowledgeTool(searchService)
            ])
            .Build();
    }
}
```

**Done when**: Agent can analyze poll results and store insights.

---

## Component 12: knowledge-curator-agent

### File to create

**`src/ConferenceAssistant.Agents/KnowledgeCuratorAgent.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Tools;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;
using ConferenceAssistant.Ingestion.Pipelines;

namespace ConferenceAssistant.Agents;

public static class KnowledgeCuratorAgent
{
    public const string Name = "KnowledgeCurator";

    public const string Instructions = """
        You are the Knowledge Curator. Your job is to find and synthesize knowledge from the session's vector store and external sources.

        When answering questions or enriching insights:
        1. First, search the local knowledge base with search_knowledge.
        2. If local knowledge is insufficient, you may suggest fetching from external MCP sources.
        3. When you receive external content, use ingest_content to add it to the knowledge base.
        4. Synthesize information from multiple sources into clear, concise answers.

        You are the RAG specialist. Combine local session knowledge with external documentation to give the best possible answers.
        """;

    public static ChatClientAgent Create(
        IChatClient chatClient,
        SemanticSearchService searchService,
        McpContentIngestionPipeline mcpPipeline)
    {
        return new ChatClientAgentBuilder()
            .WithChatClient(chatClient)
            .WithName(Name)
            .WithInstructions(Instructions)
            .WithTools([
                KnowledgeTools.CreateSearchKnowledgeTool(searchService),
                KnowledgeTools.CreateIngestContentTool(mcpPipeline)
            ])
            .Build();
    }
}
```

**Done when**: Agent can search knowledge and ingest external content.

---

## Component 13: poll-generation-workflow

### File to create

**`src/ConferenceAssistant.Agents/Workflows/PollGenerationWorkflow.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace ConferenceAssistant.Agents.Workflows;

public class PollGenerationWorkflow
{
    private readonly ChatClientAgent _surveyArchitect;

    public PollGenerationWorkflow(ChatClientAgent surveyArchitect)
    {
        _surveyArchitect = surveyArchitect;
    }

    public async Task<string> GeneratePollAsync(string topicId, string topicTitle, string? pollHint = null)
    {
        var prompt = $"""
            Generate a poll for the current topic: "{topicTitle}" (ID: {topicId}).
            {(pollHint is not null ? $"Hint: {pollHint}" : "")}
            First search the knowledge base for context, then create the poll.
            """;

        var result = await _surveyArchitect.RunAsync(prompt);
        return result;
    }
}
```

> **Note**: The exact `RunAsync` API depends on the Agent Framework version. It may be `agent.RunAsync(prompt)` returning a string, or it may use `AgentSession` / `AgentWorkflowBuilder`. Adapt to the actual API. The pattern is: send a prompt to the Survey Architect agent, which uses its tools (search_knowledge, create_poll) to produce a poll.

**Done when**: Calling `GeneratePollAsync` triggers the agent, which searches the knowledge base and creates a poll.

---

## Component 14: response-analysis-workflow

### File to create

**`src/ConferenceAssistant.Agents/Workflows/ResponseAnalysisWorkflow.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace ConferenceAssistant.Agents.Workflows;

public class ResponseAnalysisWorkflow
{
    private readonly ChatClientAgent _responseAnalyst;
    private readonly ChatClientAgent _knowledgeCurator;

    public ResponseAnalysisWorkflow(
        ChatClientAgent responseAnalyst,
        ChatClientAgent knowledgeCurator)
    {
        _responseAnalyst = responseAnalyst;
        _knowledgeCurator = knowledgeCurator;
    }

    public async Task<string> AnalyzeAsync(string pollId)
    {
        // Step 1: Response Analyst interprets the results
        var analysis = await _responseAnalyst.RunAsync(
            $"Analyze the results of poll {pollId}. Get the results, search for context, and store your insight.");

        // Step 2: Knowledge Curator enriches with supporting docs
        var enriched = await _knowledgeCurator.RunAsync(
            $"The Response Analyst just analyzed poll {pollId} and found: {analysis}. " +
            "Search the knowledge base for supporting content. If helpful, suggest external sources.");

        return enriched;
    }
}
```

**Done when**: Workflow runs analyst first, then curator. Insight gets stored.

---

## Component 15: session-summary-workflow

### File to create

**`src/ConferenceAssistant.Agents/Workflows/SessionSummaryWorkflow.cs`**:
```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Tools;
using ConferenceAssistant.Core.Services;

namespace ConferenceAssistant.Agents.Workflows;

public class SessionSummaryWorkflow
{
    private readonly IChatClient _chatClient;
    private readonly SessionService _sessionService;
    private readonly PollService _pollService;
    private readonly ChatClientAgent _surveyArchitect;
    private readonly ChatClientAgent _responseAnalyst;
    private readonly ChatClientAgent _knowledgeCurator;

    public SessionSummaryWorkflow(
        IChatClient chatClient,
        SessionService sessionService,
        PollService pollService,
        ChatClientAgent surveyArchitect,
        ChatClientAgent responseAnalyst,
        ChatClientAgent knowledgeCurator)
    {
        _chatClient = chatClient;
        _sessionService = sessionService;
        _pollService = pollService;
        _surveyArchitect = surveyArchitect;
        _responseAnalyst = responseAnalyst;
        _knowledgeCurator = knowledgeCurator;
    }

    public async Task<string> GenerateSummaryAsync(CancellationToken ct = default)
    {
        // Gather data from each agent's perspective
        var getSummaryData = InsightTools.CreateGetSessionSummaryDataTool(_sessionService, _pollService);
        var dataResult = await getSummaryData.InvokeAsync([], ct);
        var sessionData = dataResult?.ToString() ?? "";

        // Get Survey Architect's perspective on audience engagement
        var surveyPerspective = await _surveyArchitect.RunAsync(
            $"Based on this session data, what patterns do you see in how the polls performed? " +
            $"Which topics generated the most engagement?\n\n{sessionData}");

        // Get Response Analyst's perspective on audience understanding
        var analystPerspective = await _responseAnalyst.RunAsync(
            $"Based on this session data, what do the poll results tell us about the audience's " +
            $"understanding of the topics? Any knowledge gaps or surprises?\n\n{sessionData}");

        // Get Knowledge Curator's perspective on content connections
        var curatorPerspective = await _knowledgeCurator.RunAsync(
            "Search the knowledge base broadly and identify the key themes and connections " +
            "across all ingested content from this session.");

        // Synthesize all perspectives into a final summary
        var synthesisPrompt = $"""
            You are creating the closing summary for a live conference session. Synthesize the following
            perspectives from three AI agents into a comprehensive, engaging summary:

            ## Session Data
            {sessionData}

            ## Survey Architect's Perspective (Engagement)
            {surveyPerspective}

            ## Response Analyst's Perspective (Understanding)
            {analystPerspective}

            ## Knowledge Curator's Perspective (Themes)
            {curatorPerspective}

            Write a summary that covers:
            1. Session overview and topics covered
            2. Key audience insights from polls
            3. Most discussed themes
            4. Knowledge gaps identified
            5. Memorable moments
            6. Key takeaways

            Make it engaging and personal — this was a live, interactive session.
            """;

        var response = await _chatClient.GetResponseAsync(synthesisPrompt, ct);
        return response.Text ?? "Summary generation failed.";
    }
}
```

**`src/ConferenceAssistant.Agents/DependencyInjection.cs`**:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;
using ConferenceAssistant.Ingestion.Pipelines;
using ConferenceAssistant.Agents.Workflows;

namespace ConferenceAssistant.Agents;

public static class AgentsServiceCollectionExtensions
{
    public static IServiceCollection AddAgentServices(this IServiceCollection services)
    {
        // Agent instances (singleton — they're stateless)
        services.AddSingleton(sp => SurveyArchitectAgent.Create(
            sp.GetRequiredService<IChatClient>(),
            sp.GetRequiredService<PollService>(),
            sp.GetRequiredService<SemanticSearchService>()));

        services.AddSingleton(sp => ResponseAnalystAgent.Create(
            sp.GetRequiredService<IChatClient>(),
            sp.GetRequiredService<PollService>(),
            sp.GetRequiredService<SessionService>(),
            sp.GetRequiredService<SemanticSearchService>()));

        services.AddSingleton(sp => KnowledgeCuratorAgent.Create(
            sp.GetRequiredService<IChatClient>(),
            sp.GetRequiredService<SemanticSearchService>(),
            sp.GetRequiredService<McpContentIngestionPipeline>()));

        // Workflows
        services.AddSingleton<PollGenerationWorkflow>(sp =>
            new PollGenerationWorkflow(
                sp.GetRequiredService<Microsoft.Agents.AI.ChatClientAgent>()));

        services.AddSingleton<ResponseAnalysisWorkflow>();
        services.AddSingleton<SessionSummaryWorkflow>();

        return services;
    }
}
```

> **Note on DI**: The agent DI registration above is simplified. Since we have three `ChatClientAgent` instances with different names, you may need to use keyed services or a factory pattern to resolve them by name. The key insight is that each agent static class produces a differently-configured `ChatClientAgent`. Use named registrations or a custom factory.

**Done when**: `SessionSummaryWorkflow.GenerateSummaryAsync()` gathers data, gets perspectives from all agents, and synthesizes a comprehensive summary.

---

## Component 16: mcp-server-tools

### File to create

**`src/ConferenceAssistant.Mcp/Server/ConferenceTools.cs`**:
```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;
using ConferenceAssistant.Agents.Workflows;

namespace ConferenceAssistant.Mcp.Server;

[McpServerToolType]
public class ConferenceTools(
    PollService pollService,
    SessionService sessionService,
    SemanticSearchService searchService,
    SessionSummaryWorkflow summaryWorkflow)
{
    [McpServerTool, Description("Get all currently active polls with their questions and options")]
    public IReadOnlyList<Poll> GetActivePolls() => pollService.GetActivePolls();

    [McpServerTool, Description("Submit a vote on an active poll")]
    public string SubmitPollResponse(string pollId, string selectedOption, string? attendeeId = null)
    {
        try
        {
            pollService.SubmitResponse(pollId, selectedOption, attendeeId);
            return $"Vote submitted for '{selectedOption}' on poll {pollId}.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get results and AI-generated insights for a specific poll")]
    public object GetPollResults(string pollId)
    {
        var poll = pollService.GetPoll(pollId);
        if (poll is null) return new { error = "Poll not found" };

        var tally = pollService.GetResultTally(pollId);
        var insights = sessionService.GetInsightsForPoll(pollId);

        return new
        {
            poll.Question,
            poll.Status,
            Results = tally,
            TotalVotes = tally.Values.Sum(),
            Insights = insights.Select(i => i.Content).ToList()
        };
    }

    [McpServerTool, Description("Semantic search over all session knowledge including outline, responses, questions, and ingested docs")]
    public async Task<IReadOnlyList<object>> SearchSessionKnowledge(string query, int topK = 5)
    {
        var results = await searchService.SearchAsync(query, topK);
        return results.Select(r => new
        {
            r.Content,
            r.Source,
            r.Summary,
            r.Keywords
        } as object).ToList();
    }

    [McpServerTool, Description("Ask a question about the session — triggers AI-powered RAG search and answer")]
    public async Task<string> AskSessionQuestion(string question)
    {
        var results = await searchService.SearchAsync(question, 5);
        if (results.Count == 0) return "No relevant knowledge found for your question.";

        var context = string.Join("\n\n", results.Select(r => r.Content));
        return $"Based on session knowledge:\n\n{context}\n\nRelevant sources: {string.Join(", ", results.Select(r => r.Source).Distinct())}";
    }

    [McpServerTool, Description("Generate a comprehensive summary of the entire session — triggers multi-agent workflow")]
    public async Task<string> GenerateSessionSummary()
    {
        return await summaryWorkflow.GenerateSummaryAsync();
    }
}
```

**Done when**: All 6 MCP tools compile and have descriptive `[Description]` attributes.

---

## Component 17: mcp-server-resources

### File to create

**`src/ConferenceAssistant.Mcp/Server/ConferenceResources.cs`**:
```csharp
using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ConferenceAssistant.Core.Services;

namespace ConferenceAssistant.Mcp.Server;

[McpServerResourceType]
public class ConferenceResources(SessionService sessionService)
{
    [McpServerResource("conference://outline"), Description("The session outline and topics")]
    public string GetOutline()
    {
        var topics = sessionService.GetAllTopics();
        return JsonSerializer.Serialize(topics, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerResource("conference://insights"), Description("All AI-generated insights from the session")]
    public string GetInsights()
    {
        var insights = sessionService.GetInsights();
        return JsonSerializer.Serialize(insights, new JsonSerializerOptions { WriteIndented = true });
    }
}
```

**Done when**: Resources compile and return serialized session data.

---

## Component 18: mcp-clients

### File to create

**`src/ConferenceAssistant.Mcp/Clients/McpClientFactory.cs`**:
```csharp
using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace ConferenceAssistant.Mcp.Clients;

public static class McpClientFactory
{
    public static async Task<IMcpClient> CreateMicrosoftLearnClientAsync(
        CancellationToken ct = default)
    {
        return await McpClientFactory.CreateAsync(
            new McpClientOptions
            {
                ClientInfo = new() { Name = "ConferencePulse", Version = "1.0" }
            },
            new StreamableHttpClientTransport(new Uri("https://learn.microsoft.com/api/mcp")),
            ct);
    }

    public static async Task<IMcpClient> CreateDeepWikiClientAsync(
        CancellationToken ct = default)
    {
        return await McpClientFactory.CreateAsync(
            new McpClientOptions
            {
                ClientInfo = new() { Name = "ConferencePulse", Version = "1.0" }
            },
            new StreamableHttpClientTransport(new Uri("https://deepwiki.com/api/mcp")),
            ct);
    }
}
```

> **Note**: The MCP client API may differ based on the `ModelContextProtocol` package version. The key pattern is: create a client with Streamable HTTP transport pointing at the remote MCP server URL. Adapt class/method names to the actual API.

**`src/ConferenceAssistant.Mcp/DependencyInjection.cs`**:
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceAssistant.Mcp;

public static class McpServiceCollectionExtensions
{
    public static IServiceCollection AddMcpServices(this IServiceCollection services)
    {
        // MCP clients are created on-demand by the KnowledgeCurator agent
        // MCP server is configured in Program.cs via MapMcpServer()
        return services;
    }
}
```

**Done when**: MCP client factory can create clients for Microsoft Learn and DeepWiki.

---

## Component 19: copilot-demo-app

### File to create

**`src/ConferenceAssistant.CopilotDemo/Program.cs`**:
```csharp
using GitHub.Copilot.SDK;

Console.WriteLine("🎙️ Conference Pulse — Copilot SDK Closer");
Console.WriteLine("==========================================\n");

Console.WriteLine("Connecting to GitHub Copilot...");
await using var copilot = new CopilotClient();
await copilot.StartAsync();
Console.WriteLine("✅ Connected.\n");

Console.WriteLine("Configuring MCP server connection...");
var config = new SessionConfig
{
    McpServers =
    {
        ["conference-pulse"] = new McpServerConfig
        {
            Url = "http://localhost:5000/mcp"
        }
    },
    OnPermissionRequest = async (permission) =>
    {
        Console.WriteLine($"  🔑 Granting permission: {permission.Description}");
        return true;
    }
};

Console.WriteLine("✅ MCP server configured.\n");
Console.WriteLine("Asking Copilot to summarize the session...\n");
Console.WriteLine("───────────────────────────────────────────\n");

var agent = copilot.AsAIAgent(config, ownsClient: true);
await foreach (var update in agent.RunStreamingAsync(
    "Write a comprehensive summary of today's session. " +
    "Use the conference-pulse MCP tools to get all session data. " +
    "Include poll results, audience sentiment, key questions, top insights, and key takeaways. " +
    "Make it engaging — this was a live, interactive session."))
{
    Console.Write(update);
}

Console.WriteLine("\n\n───────────────────────────────────────────");
Console.WriteLine("🎤 One command. Six technologies. Working together.");
```

> **Note**: The exact `GitHub.Copilot.SDK` API (`CopilotClient`, `SessionConfig`, `McpServerConfig`, `AsAIAgent`, `RunStreamingAsync`) depends on the package version. The pattern is: create client → configure MCP servers → create agent → stream response. Adapt to the actual API surface.

**Done when**: Console app compiles. When run (with conference app running), it connects to Copilot, discovers MCP tools, and streams a session summary.

---

## Component 20–24: Blazor Frontend

### Component 20: blazor-layouts

**`src/ConferenceAssistant.Web/Components/Layout/PresentationLayout.razor`**:
```razor
@inherits LayoutComponentBase

<div class="presentation-layout">
    @Body
</div>

@code {
    // Full-screen layout for Display view (no nav, no chrome)
}
```

**`src/ConferenceAssistant.Web/Components/Layout/DashboardLayout.razor`**:
```razor
@inherits LayoutComponentBase

<div class="dashboard-layout">
    <nav class="dashboard-nav">
        <span class="brand">🎙️ Conference Pulse</span>
        <span class="session-code">Session: @SessionCode</span>
    </nav>
    <main class="dashboard-content">
        @Body
    </main>
</div>

@code {
    [CascadingParameter] public string SessionCode { get; set; } = "AICONF";
}
```

### Component 21: blazor-presenter

**`src/ConferenceAssistant.Web/Components/Pages/Presenter.razor`**:
```razor
@page "/presenter"
@layout DashboardLayout
@inject SessionStateService State

<PageTitle>Presenter Dashboard</PageTitle>

<div class="presenter-grid">
    <div class="topic-panel">
        <TopicDisplay Topic="@State.CurrentTopic" />
        <button @onclick="AdvanceTopic">Next Topic →</button>
    </div>

    <div class="poll-panel">
        <h3>Poll Control</h3>
        <button @onclick="GeneratePoll">🤖 Generate Poll</button>
        @if (State.CurrentPoll is not null)
        {
            <LivePoll Poll="@State.CurrentPoll" ShowControls="true"
                      OnLaunch="LaunchPoll" OnClose="CloseAndAnalyze" />
        }
    </div>

    <div class="insights-panel">
        <InsightPanel Insights="@State.Insights" />
    </div>

    <div class="activity-panel">
        <AgentActivityLog Activities="@State.AgentActivities" />
    </div>

    <div class="questions-panel">
        <QuestionFeed Questions="@State.Questions" ShowAnswers="true" />
    </div>
</div>

@code {
    private async Task GeneratePoll() { /* Trigger PollGenerationWorkflow */ }
    private async Task LaunchPoll() { /* PollService.LaunchPoll() */ }
    private async Task CloseAndAnalyze() { /* Close + ResponseAnalysisWorkflow */ }
    private async Task AdvanceTopic() { /* SessionService.AdvanceToNextTopic() */ }
}
```

### Component 22: blazor-session

**`src/ConferenceAssistant.Web/Components/Pages/Session.razor`**:
```razor
@page "/session/{Code}"
@inject SessionStateService State

<PageTitle>Conference Pulse</PageTitle>

<div class="session-view">
    <h2>@State.CurrentTopic?.Title</h2>

    @if (State.ActivePoll is not null)
    {
        <LivePoll Poll="@State.ActivePoll" ShowControls="false"
                  OnVote="SubmitVote" />
    }
    else
    {
        <p class="waiting">Waiting for next poll...</p>
    }

    <div class="question-input">
        <input @bind="newQuestion" placeholder="Ask a question..." />
        <button @onclick="AskQuestion">Ask</button>
    </div>

    <QuestionFeed Questions="@State.Questions" OnUpvote="UpvoteQuestion" />
</div>

@code {
    [Parameter] public string Code { get; set; } = "";
    private string newQuestion = "";

    private async Task SubmitVote(string option) { /* PollService.SubmitResponse() */ }
    private async Task AskQuestion() { /* SessionService.AddQuestion() */ }
    private async Task UpvoteQuestion(string id) { /* SessionService.UpvoteQuestion() */ }
}
```

### Component 23: blazor-display

**`src/ConferenceAssistant.Web/Components/Pages/Display.razor`**:
```razor
@page "/display"
@layout PresentationLayout
@inject SessionStateService State

<div class="display-grid">
    <div class="main-content">
        <TopicDisplay Topic="@State.CurrentTopic" Large="true" />

        @if (State.ActivePoll is not null)
        {
            <PollResults Poll="@State.ActivePoll"
                         Tally="@State.CurrentTally"
                         Animated="true" />
        }

        @if (State.StreamingSummary is not null)
        {
            <div class="streaming-summary">
                <h2>Session Summary</h2>
                <p>@State.StreamingSummary</p>
            </div>
        }
    </div>

    <div class="sidebar">
        <InsightPanel Insights="@State.Insights" />
        <QuestionFeed Questions="@State.Questions.Take(5).ToList()" Compact="true" />
    </div>
</div>
```

### Component 24: blazor-components (Shared)

Create these files in `src/ConferenceAssistant.Web/Components/Shared/`:

- **`TopicDisplay.razor`**: Shows topic title, description, talking points. `[Parameter] SessionTopic? Topic`, `[Parameter] bool Large`
- **`LivePoll.razor`**: Poll question + option buttons (for voting) OR results (after voting). `[Parameter] Poll Poll`, `[Parameter] bool ShowControls`, `[Parameter] EventCallback<string> OnVote`
- **`PollResults.razor`**: Animated horizontal bar chart. `[Parameter] Poll Poll`, `[Parameter] Dictionary<string, int> Tally`, `[Parameter] bool Animated`
- **`InsightPanel.razor`**: List of AI insights with icons per type. `[Parameter] IReadOnlyList<Insight> Insights`
- **`QuestionFeed.razor`**: List of questions with upvote buttons. `[Parameter] IReadOnlyList<AudienceQuestion> Questions`, `[Parameter] EventCallback<string> OnUpvote`, `[Parameter] bool Compact`
- **`AgentActivityLog.razor`**: Live activity log showing agent actions. `[Parameter] IReadOnlyList<string> Activities`

Each component should subscribe to `SessionStateService` events and call `InvokeAsync(StateHasChanged)` when data changes.

**Done when**: All Blazor pages render. Presenter can trigger polls. Session can vote. Display shows results.

---

## Component 25: program-cs-wiring

### File to modify

**`src/ConferenceAssistant.Web/Program.cs`**:
```csharp
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion;
using ConferenceAssistant.Agents;
using ConferenceAssistant.Mcp;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- M.E.AI: IChatClient + IEmbeddingGenerator ---
var azureEndpoint = builder.Configuration["AzureOpenAI:Endpoint"]
    ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Azure OpenAI endpoint not configured");
var azureKey = builder.Configuration["AzureOpenAI:ApiKey"]
    ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Azure OpenAI API key not configured");
var chatDeployment = builder.Configuration["AzureOpenAI:ChatDeployment"] ?? "gpt-4o";
var embeddingDeployment = builder.Configuration["AzureOpenAI:EmbeddingDeployment"] ?? "text-embedding-3-small";

var azureClient = new AzureOpenAIClient(new Uri(azureEndpoint), new System.ClientModel.ApiKeyCredential(azureKey));

builder.Services.AddChatClient(azureClient.GetChatClient(chatDeployment).AsIChatClient())
    .UseFunctionInvocation()
    .UseOpenTelemetry()
    .Build();

builder.Services.AddEmbeddingGenerator(
    azureClient.GetEmbeddingClient(embeddingDeployment).AsIEmbeddingGenerator());

// --- Core Services ---
builder.Services.AddSingleton<PollService>();
builder.Services.AddSingleton<SessionService>();

// --- Ingestion + VectorData ---
builder.Services.AddIngestionServices();

// --- Agents ---
builder.Services.AddAgentServices();

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- MCP Server ---
builder.Services.AddMcpServer()
    .WithTools<ConferenceAssistant.Mcp.Server.ConferenceTools>();

var app = builder.Build();

// --- Initialize session data ---
using (var scope = app.Services.CreateScope())
{
    var sessionService = scope.ServiceProvider.GetRequiredService<SessionService>();
    await sessionService.LoadTopicsAsync(
        builder.Configuration["Session:TopicsPath"] ?? "data/seed-topics.json");

    // Ingest session outline at startup
    var outlinePipeline = scope.ServiceProvider.GetRequiredService<
        ConferenceAssistant.Ingestion.Pipelines.OutlineIngestionPipeline>();
    var outlinePath = builder.Configuration["Session:OutlinePath"] ?? "data/session-outline.md";
    if (File.Exists(outlinePath))
    {
        await outlinePipeline.IngestAsync(outlinePath);
    }
}

app.UseStaticFiles();
app.UseAntiforgery();

// MCP endpoint
app.MapMcp("/mcp");

// Blazor
app.MapRazorComponents<ConferenceAssistant.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

**Done when**: App starts, loads topics, ingests outline, serves Blazor pages, and exposes MCP endpoint.

---

## Component 26: session-data

Already created in `/data/seed-topics.json` and `/docs/session-outline.md`. Copy `docs/session-outline.md` to `data/session-outline.md` for the app to ingest.

---

## Component 27: aspire-apphost

**`src/ConferenceAssistant.AppHost/Program.cs`**:
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.ConferenceAssistant_Web>("web");

builder.Build().Run();
```

**Done when**: `dotnet run --project src/ConferenceAssistant.AppHost` starts the app with Aspire dashboard.

---

## Component 28: readme-docs

Update `README.md` at repo root with:
- Project description
- Architecture overview (link to docs/)
- Prerequisites: .NET 10 SDK, Azure OpenAI access, GitHub Copilot CLI (for closer)
- Quick start: `dotnet run --project src/ConferenceAssistant.AppHost`
- Environment variables
- Demo script (abbreviated)
- Links to docs/

**Done when**: A new developer can clone, read README, set env vars, and run the app.

---

## Component 29: slide-model

### What to create

**`src/ConferenceAssistant.Core/Models/Slide.cs`** — Domain model for individual presentation slides plus supporting enums:

```csharp
namespace ConferenceAssistant.Core.Models;

public class Slide
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? TopicId { get; set; }
    public int Order { get; set; }
    public SlideType Type { get; set; } = SlideType.Content;
    public SlideLayout Layout { get; set; } = SlideLayout.Default;
    public string Title { get; set; } = "";
    public string? Subtitle { get; set; }
    public List<string> Bullets { get; set; } = [];
    public string? BodyMarkdown { get; set; }
    public string? CodeSnippet { get; set; }
    public string? CodeLanguage { get; set; }
    public string SpeakerNotes { get; set; } = "";
}

public enum SlideType
{
    Title,
    Content,
    Code,
    Section,
    Poll,
    Blank
}

public enum SlideLayout
{
    Default,
    Centered,
    TwoColumn
}
```

`SessionTopic` gains a `Slides` property to hold the parsed slides for each topic:

```csharp
public List<Slide> Slides { get; set; } = [];
```

**Done when**: `dotnet build` succeeds, `Slide` model has all properties (`Id`, `TopicId`, `Order`, `Type`, `Layout`, `Title`, `Subtitle`, `Bullets`, `BodyMarkdown`, `CodeSnippet`, `CodeLanguage`, `SpeakerNotes`), `SlideType` has six members, `SlideLayout` has three members, and `SessionTopic.Slides` is populated by the parser.

---

## Component 30: slide-markdown-parser

### What to create

**`src/ConferenceAssistant.Core/Services/SlideMarkdownParser.cs`** — A static parser that converts a Markdown presentation deck into a `List<Slide>`.

**Public API:**
- `static List<Slide> Parse(string markdown)` — Synchronous parse of a markdown string.
- `static async Task<List<Slide>> ParseFileAsync(string filePath)` — Reads a file then delegates to `Parse`.

**Parsing algorithm:**
1. **Normalize** line endings to `\n`.
2. **Split** the document on lines that are exactly `---` (`Regex.Split(markdown, @"^---\s*$", RegexOptions.Multiline)`).
3. **Detect YAML frontmatter** — if the document starts with `---`, skip the first two parts (empty prefix + frontmatter body).
4. **For each block**, extract metadata via compiled regexes, then classify:
   - **Topic** (`<!-- topic: id -->`) — sticky across subsequent slides until changed.
   - **Layout** (`<!-- layout: centered|two-column -->`) — per-slide override.
   - **Speaker notes** (`<!-- speaker: ... -->`) — extracted and stripped from visible content.
   - **Blank detection** — if no visible content remains after stripping comments, emit `SlideType.Blank` (only if speaker notes were present; otherwise skip the block).
5. **Classify** the slide based on visible content:
   - `H1` + `H2` only → `SlideType.Title` (layout defaults to `Centered`).
   - `H1` only → `SlideType.Section`.
   - Heading + fenced code block → `SlideType.Code` (captures language + snippet).
   - Heading + bullet list → `SlideType.Content` (captures bullet text).
   - Heading + other body → `SlideType.Content` with `BodyMarkdown`.
   - No heading + code → `SlideType.Code`.
   - No heading + bullets → `SlideType.Content`.
   - Fallback → `SlideType.Content` with `BodyMarkdown`.

**Key regex patterns from the source:**
```csharp
private static readonly Regex SpeakerNotesRegex = new(
    @"<!--\s*speaker\s*:(.*?)-->",
    RegexOptions.Singleline | RegexOptions.Compiled);

private static readonly Regex TopicRegex = new(
    @"<!--\s*topic\s*:\s*(.*?)\s*-->",
    RegexOptions.Compiled);

private static readonly Regex LayoutRegex = new(
    @"<!--\s*layout\s*:\s*(.*?)\s*-->",
    RegexOptions.Compiled);

private static readonly Regex FencedCodeBlockRegex = new(
    @"^```(\w*)\s*\n(.*?)^```",
    RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

private static readonly Regex H1Regex = new(
    @"^#\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

private static readonly Regex H2Regex = new(
    @"^##\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

private static readonly Regex BulletRegex = new(
    @"^[\-\*]\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);
```

**Done when**: `SlideMarkdownParser.Parse(testMarkdown)` produces the correct number of `Slide` objects from a test markdown input, each with the right `Type`, `TopicId`, `Title`, `SpeakerNotes`, and `CodeSnippet` values.

---

## Component 31: slides-markdown-file

### What to create

**`data/slides.md`** — The Markdown presentation deck consumed by `SlideMarkdownParser`.

**Structure:**
- Begins with **YAML frontmatter** (between `---` fences) containing title, subtitle, and date metadata.
- Individual slides are separated by `---` on their own line.
- ~28 slides spanning 5 topics (meai, knowledge, agents, mcp, closer) plus intro/outro slides.
- Conventions used throughout:
  - `<!-- topic: <id> -->` to map a slide (and subsequent slides) to a `SessionTopic`.
  - `<!-- speaker: ... -->` for speaker notes (timing cues, demo instructions, transitions). These are only shown on the Presenter page.
  - `<!-- layout: centered -->` or `<!-- layout: two-column -->` for explicit layout overrides.
  - `# Heading` for slide titles, `## Subtitle` for subtitles, fenced code blocks for code slides, and `- bullet` lists for content slides.

**Done when**: `data/slides.md` exists, contains `---` slide separators, includes `<!-- topic: -->` mappings for all five topic IDs, and `SlideMarkdownParser.ParseFileAsync("data/slides.md")` returns ~28 slides.

---

## Component 32: session-service-slides

### What to create

Extend the session service to manage slide state and navigation.

**`src/ConferenceAssistant.Core/Services/ISessionService.cs`** — New interface members:
```csharp
event Action<Slide>? SlideChanged;

Slide? ActiveSlide { get; }
int ActiveSlideIndex { get; }
int TotalSlides { get; }

Task LoadSlidesAsync(string slidesPath);
List<Slide> GetSlidesForTopic(string topicId);
Task AdvanceSlideAsync();
Task GoBackSlideAsync();
Task GoToSlideAsync(string slideId);
Slide? GetNextSlide();
Slide? GetPreviousSlide();
```

**`src/ConferenceAssistant.Core/Services/SessionService.cs`** — Implementation details:
- `LoadSlidesAsync` calls `SlideMarkdownParser.ParseFileAsync`, stores the full slide list, and distributes slides to each `SessionTopic.Slides` by matching `TopicId`.
- `ActivateTopicAsync` auto-advances to the **first slide** of the newly activated topic (fires `SlideChanged`).
- `AdvanceSlideAsync` / `GoBackSlideAsync` move the `ActiveSlideIndex` within the global slide list and fire `SlideChanged`.
- `GoToSlideAsync` jumps to a specific slide by ID.
- `GetNextSlide` / `GetPreviousSlide` return peek values without mutating state.

**Startup wiring** — `LoadSlidesAsync` is called during app initialization in `Program.cs`, right after `LoadTopicsAsync`:
```csharp
await sessionService.LoadSlidesAsync(
    builder.Configuration["Session:SlidesPath"] ?? "data/slides.md");
```

**Done when**: Slides load at startup (log message: "Slides loaded: N slides"), navigation methods advance/retreat correctly, `SlideChanged` event fires on every navigation, and activating a topic auto-advances to that topic's first slide.

---

## Component 33: slide-renderer

### What to create

**`src/ConferenceAssistant.Web/Components/Shared/SlideRenderer.razor`** — Reusable Blazor component that renders a single `Slide`.

**Parameters:**
- `[Parameter] Slide? Slide` — the slide to render.
- `[Parameter] bool Compact` — when `true`, uses smaller fonts and tighter spacing (for presenter preview / "Up Next" thumbnail); when `false`, uses large display-mode fonts.

**Rendering by SlideType:**
| SlideType | Rendering |
|-----------|-----------|
| `Title` | Large centered title + subtitle |
| `Section` | Full-width section heading |
| `Content` | Title + bullet list or body markdown |
| `Code` | Title + syntax-highlighted code block (`<pre><code>`) with language class |
| `Blank` | Empty visible area (speaker notes only — not rendered here) |
| `Poll` | Placeholder / "Poll active" indicator |

- Display mode: large fonts, dark background, high contrast — readable from the back of a conference room.
- Compact mode: smaller fonts, contained within a preview box.

**Done when**: `<SlideRenderer Slide="@slide" />` renders each slide type correctly in both compact and full-size modes.

---

## Component 34: display-slides

### What to create

Update **`src/ConferenceAssistant.Web/Components/Pages/Display.razor`** to show slides.

**Content priority** (highest to lowest):
1. **Active Poll** — `PollResultsChart` overlays everything when a poll is live.
2. **Active Slide** — `<SlideRenderer Slide="@ActiveSlide" />` fills the main area.
3. **Latest Insight** — `InsightCard` for the most recent AI insight.
4. **Cascade visualization** — Fallback ambient display.

**Additional UI:**
- **Progress dots** at the bottom of the screen — one dot per slide in the current topic, with the active slide highlighted. Provides a subtle position indicator for the audience.

**Real-time updates:**
- Subscribe to `SlideChanged` event from `SessionStateService` → call `InvokeAsync(StateHasChanged)`.
- When a poll becomes active, it takes priority over the slide.
- When the poll closes, the slide returns.

**Done when**: Display shows the current slide full-screen, active polls overlay the slide, progress dots reflect the current position, and all transitions happen in real-time.

---

## Component 35: presenter-slides

### What to create

Update **`src/ConferenceAssistant.Web/Components/Pages/Presenter.razor`** to add slide controls.

**New UI elements in the main panel:**
- **Slide preview box** — `<SlideRenderer Slide="@ActiveSlide" Compact="true" />` showing the current slide in compact mode.
- **Speaker notes panel** — Rendered below the preview with a 🎤 icon. Shows `ActiveSlide.SpeakerNotes` (timing cues, demo instructions). Only visible on the Presenter page — never sent to the Display.
- **Navigation controls:**
  - **◀ Previous** / **Next ▶** buttons for slide navigation.
  - **Keyboard shortcuts:** `→` (Right Arrow) and `Space` advance the slide; `←` (Left Arrow) goes back. Keyboard events are captured at the panel level and suppressed when focus is inside a text input (to avoid conflicts with poll question / answer forms).
- **"Up Next" preview** — A small thumbnail using `<SlideRenderer Slide="@NextSlide" Compact="true" />` so the speaker can see what's coming.
- **Progress indicator** — "Slide X of Y" text showing `ActiveSlideIndex + 1` of `TotalSlides`.

**Done when**: Presenter shows the current slide preview, speaker notes, Previous/Next buttons, keyboard navigation (→/Space/←), "Up Next" thumbnail, and "Slide X of Y" progress.
