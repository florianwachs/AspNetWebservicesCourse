# Conference Pulse — Architecture

## System Architecture

```
┌────────────────────────────────────────────────────────────┐
│         Blazor Interactive Server (.NET 10)                 │
│                                                            │
│  /                          → Session hub (list + create)  │
│  /create                    → Create new session form      │
│  /presenter/{SessionCode}   → Speaker 3-column dashboard   │
│  /session/{SessionCode}     → Attendee participation       │
│  /display/{SessionCode}     → Projection view + QR code    │
│  slides.md                  → Markdown slide deck           │
│                                                            │
│  Real-time: SignalR (built into Blazor Server circuits)    │
└──────────────────────┬─────────────────────────────────────┘
                       │
┌──────────────────────▼─────────────────────────────────────┐
│         ASP.NET Core Host                                   │
│                                                            │
│  ┌── Agent Framework ────────────────────────────────┐     │
│  │  ChatClientAgents + AgentWorkflowBuilder          │     │
│  │  PollGeneration, ResponseAnalysis, SessionSummary │     │
│  └───────────────────────────────────────────────────┘     │
│                                                            │
│  ┌── M.E.AI ─────────────────────────────────────────┐     │
│  │  IChatClient → ChatClientBuilder pipeline         │     │
│  │  IEmbeddingGenerator, AIFunctionFactory           │     │
│  └───────────────────────────────────────────────────┘     │
│                                                            │
│  ┌── DataIngestion ──────────────────────────────────┐     │
│  │  IngestionPipeline<ConferenceRecord>              │     │
│  │  MarkdigReader, Chunkers, Enrichers, Writer       │     │
│  └───────────────────────────────────────────────────┘     │
│                                                            │
│  ┌── VectorData ─────────────────────────────────────┐     │
│  │  QdrantVectorStore (Qdrant, auto-embedding)       │     │
│  │  VectorStoreCollection + SearchAsync()            │     │
│  └───────────────────────────────────────────────────┘     │
│                                                            │
│  ┌── Persistence ────────────────────────────────────┐     │
│  │  ConferenceDbContext (EF Core + PostgreSQL)        │     │
│  │  SessionPersistenceService (save-on-mutation)      │     │
│  └───────────────────────────────────────────────────┘     │
│                                                            │
│  ┌── MCP ────────────────────────────────────────────┐     │
│  │  SERVER (/mcp): 10 tools (8 conference + 2 KB)    │     │
│  │  CLIENTS: Microsoft Learn, DeepWiki               │     │
│  └───────────────────────────────────────────────────┘     │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│  Copilot SDK Demo (console app) — planned, not yet impl'd  │
│  CopilotClient → McpServers → "Summarize this session"     │
└────────────────────────────────────────────────────────────┘
```

---

## Project Structure

```
dotnet-ai-conference-assistant/
├── src/
│   ├── ConferenceAssistant.AppHost/              # .NET Aspire AppHost
│   │   ├── ConferenceAssistant.AppHost.csproj
│   │   └── Program.cs
│   │
│   ├── ConferenceAssistant.ServiceDefaults/      # Aspire service defaults
│   │   ├── ConferenceAssistant.ServiceDefaults.csproj
│   │   └── Extensions.cs
│   │
│   ├── ConferenceAssistant.Web/                  # ASP.NET Core + Blazor Server
│   │   ├── ConferenceAssistant.Web.csproj
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── Components/
│   │   │   ├── App.razor
│   │   │   ├── Routes.razor
│   │   │   ├── _Imports.razor
│   │   │   ├── Layout/
│   │   │   │   ├── PresentationLayout.razor
│   │   │   │   ├── PresentationLayout.razor.css
│   │   │   │   ├── HomeLayout.razor
│   │   │   │   ├── DashboardLayout.razor
│   │   │   │   └── DashboardLayout.razor.css
│   │   │   ├── Pages/
│   │   │   │   ├── Home.razor
│   │   │   │   ├── CreateSession.razor
│   │   │   │   ├── Presenter.razor
│   │   │   │   ├── Session.razor
│   │   │   │   └── Display.razor
│   │   │   └── Shared/
│   │   │       ├── TopicDisplay.razor
│   │   │       ├── LivePoll.razor
│   │   │       ├── PollResults.razor
│   │   │       ├── InsightPanel.razor
│   │   │       ├── QuestionFeed.razor
│   │   │       ├── AgentActivityLog.razor
│   │   │       └── SlideRenderer.razor
│   │   ├── Data/
│   │   │   └── ConferenceDbContext.cs
│   │   ├── Services/
│   │   │   ├── SessionStateService.cs
│   │   │   ├── SessionPersistenceService.cs
│   │   │   └── ISessionPersistenceService.cs
│   │   └── wwwroot/
│   │       └── css/
│   │           └── app.css
│   │
│   ├── ConferenceAssistant.CopilotDemo/          # Copilot SDK closer (planned, not yet implemented)
│   │
│   ├── ConferenceAssistant.Agents/               # Agent Framework layer
│   │   ├── ConferenceAssistant.Agents.csproj
│   │   ├── SurveyArchitectAgent.cs
│   │   ├── ResponseAnalystAgent.cs
│   │   ├── KnowledgeCuratorAgent.cs
│   │   ├── Tools/
│   │   │   ├── PollTools.cs
│   │   │   ├── KnowledgeTools.cs
│   │   │   └── InsightTools.cs
│   │   ├── Workflows/
│   │   │   ├── PollGenerationWorkflow.cs
│   │   │   ├── ResponseAnalysisWorkflow.cs
│   │   │   └── SessionSummaryWorkflow.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── ConferenceAssistant.Ingestion/            # DataIngestion pipelines
│   │   ├── ConferenceAssistant.Ingestion.csproj
│   │   ├── Models/
│   │   │   └── ConferenceRecord.cs
│   │   ├── Pipelines/
│   │   │   ├── OutlineIngestionPipeline.cs
│   │   │   ├── ResponseIngestionPipeline.cs
│   │   │   └── McpContentIngestionPipeline.cs
│   │   ├── Readers/
│   │   │   └── TextContentReader.cs
│   │   ├── Services/
│   │   │   ├── SemanticSearchService.cs
│   │   │   ├── ISemanticSearchService.cs
│   │   │   ├── IIngestionService.cs
│   │   │   ├── IngestionService.cs
│   │   │   └── IIngestionTracker.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── ConferenceAssistant.Mcp/                  # MCP server + clients
│   │   ├── ConferenceAssistant.Mcp.csproj
│   │   ├── Server/
│   │   │   └── ConferenceResources.cs
│   │   ├── Tools/
│   │   │   ├── ConferenceTools.cs
│   │   │   └── KnowledgeTools.cs
│   │   ├── Clients/
│   │   │   └── McpClientFactory.cs
│   │   └── DependencyInjection.cs
│   │
│   └── ConferenceAssistant.Core/                 # Shared domain
│       ├── ConferenceAssistant.Core.csproj
│       ├── Models/
│       │   ├── ConferenceSession.cs
│       │   ├── SessionContext.cs
│       │   ├── Poll.cs
│       │   ├── PollResponse.cs
│       │   ├── SessionTopic.cs
│       │   ├── Insight.cs
│       │   └── AudienceQuestion.cs
│       └── Services/
│           ├── ISessionManager.cs
│           ├── SessionManager.cs
│           ├── PollService.cs
│           ├── SessionService.cs
│           └── SlideMarkdownParser.cs
│
├── data/
│   ├── session-outline.md
│   ├── seed-topics.json
│   └── slides.md
│
├── tests/
│   ├── ConferenceAssistant.Agents.Tests/
│   │   └── ConferenceAssistant.Agents.Tests.csproj
│   ├── ConferenceAssistant.Ingestion.Tests/
│   │   └── ConferenceAssistant.Ingestion.Tests.csproj
│   └── ConferenceAssistant.Mcp.Tests/
│       └── ConferenceAssistant.Mcp.Tests.csproj
│
├── docs/
│   ├── plan.md
│   ├── prd.md
│   ├── architecture.md
│   ├── implementation-spec.md
│   ├── session-outline.md
│   └── slide-authoring-guide.md
│
├── Directory.Packages.props
├── Directory.Build.props
├── ConferenceAssistant.sln
├── README.md
└── .gitignore
```

---

## Multi-Session Architecture

The app supports multiple concurrent sessions with Jitsi-style host PIN protection. Each session is fully isolated with its own state, polls, questions, insights, and event pipeline.

### Session Lifecycle

```
User visits /create
  → Fills in title, code (optional), PIN, description, template
    → SessionManager.CreateSession()
      → New SessionContext created + stored in ConcurrentDictionary
        → SessionCreated event fires
          → AI pipelines (Q&A auto-answer, insight generation, ingestion) wired to new session
            → Session appears on home page (/)
```

### SessionContext — Per-Session Aggregate

`SessionContext` (`ConferenceAssistant.Core/Models/SessionContext.cs`) holds **all state** for a single session:

- `ConferenceSession` — metadata (title, code, PIN, status, created timestamp)
- `Slides` — parsed slide deck
- `Polls` — active and historical polls
- `Questions` — audience Q&A
- `Insights` — AI-generated analysis
- `Events` — per-session event handlers (OnPollCreated, OnQuestionAsked, etc.)

This aggregate pattern ensures sessions are fully isolated — no shared mutable state between sessions.

### SessionManager — Singleton Lifecycle Manager

`ISessionManager` / `SessionManager` (`ConferenceAssistant.Core/Services/`) manages the collection of active sessions:

```csharp
// Simplified interface
public interface ISessionManager
{
    SessionContext CreateSession(string title, string hostPin, string? code = null, ...);
    SessionContext? GetSession(string sessionCode);
    IReadOnlyList<SessionInfo> GetActiveSessions();
    event Action<SessionContext>? OnSessionCreated;
}
```

- Backed by `ConcurrentDictionary<string, SessionContext>` for thread-safe multi-session access
- `SessionInfo` is a lightweight record (code, title, status, attendee count) for the home page listing
- `OnSessionCreated` event is the hook for wiring AI pipelines to new sessions

### Event Wiring via SessionCreated

When `SessionCreated` fires, the host (`Program.cs`) subscribes AI services to the new session's events:

- **Q&A auto-answer** — listens to `SessionContext.OnQuestionAsked`
- **Insight generation** — listens to `SessionContext.OnPollClosed` and `SessionContext.OnTopicCompleted`
- **Real-time ingestion** — listens to `SessionContext.OnPollClosed`, `SessionContext.OnQuestionAnswered`, `SessionContext.OnInsightGenerated`

This means AI features work automatically for **all** sessions, not just the default.

### PIN Gate Protection Model

The presenter dashboard (`/presenter/{SessionCode}`) is protected by a host PIN:

1. **Creator sets PIN** — 4-6 digit numeric PIN during session creation
2. **PIN gate UI** — navigating to `/presenter/{code}` shows a PIN input form
3. **Validation** — entered PIN compared against `ConferenceSession.HostPin`
4. **Per-circuit state** — PIN validation is stored in Blazor component state (per browser tab)
5. **No infrastructure** — no cookies, tokens, sessions, or middleware involved

The display (`/display/{SessionCode}`) and attendee (`/session/{SessionCode}`) views have no PIN gate.

### Default Demo Session

At startup, a default session is auto-created from `data/seed-topics.json`:
- PIN: `0000`
- Session code: auto-generated (logged at startup)
- Full AI pipeline wired automatically

### Backward Compatibility Layer

Existing singleton services (`SessionService`, `PollService`, etc.) remain as backward-compatible wrappers:

- They delegate to the **default session** via `SessionManager`
- MCP tools and agent workflows use the same old interfaces unchanged
- Events from `SessionContext` are forwarded to global service events (e.g., `SessionStateService.OnPollCreated`)
- This means all existing MCP tools, agent workflows, and Copilot SDK integration work without modification

---

## NuGet Package Matrix

Each project lists only its DIRECT package references. Transitive dependencies are inherited.

### ConferenceAssistant.Core
```xml
<!-- No external NuGet packages — pure domain models and service interfaces -->
```

### ConferenceAssistant.Ingestion
```xml
<PackageReference Include="Microsoft.Extensions.AI" />
<PackageReference Include="Microsoft.Extensions.DataIngestion" />
<PackageReference Include="Microsoft.Extensions.DataIngestion.Markdig" />
<PackageReference Include="Microsoft.Extensions.VectorData.Abstractions" />
<PackageReference Include="Microsoft.ML.Tokenizers" />
<PackageReference Include="Microsoft.SemanticKernel.Connectors.Qdrant" />
```
Project reference: `ConferenceAssistant.Core`

### ConferenceAssistant.Agents
```xml
<PackageReference Include="Microsoft.Agents.AI" />
<PackageReference Include="Microsoft.Agents.AI.Workflows" />
<PackageReference Include="Microsoft.Extensions.AI.Abstractions" />
```
Project references: `ConferenceAssistant.Core`, `ConferenceAssistant.Ingestion`

### ConferenceAssistant.Mcp
```xml
<PackageReference Include="ModelContextProtocol" />
<PackageReference Include="ModelContextProtocol.AspNetCore" />
<PackageReference Include="Microsoft.Extensions.AI.Abstractions" />
```
Project references: `ConferenceAssistant.Core`, `ConferenceAssistant.Ingestion`, `ConferenceAssistant.Agents`

### ConferenceAssistant.Web
```xml
<PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" />
<PackageReference Include="Aspire.Qdrant.Client" />
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="Microsoft.Extensions.AI" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" />
<PackageReference Include="Aspire.Azure.AI.OpenAI" />
<PackageReference Include="ModelContextProtocol.AspNetCore" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
<PackageReference Include="QRCoder" />
```
Project references: `ConferenceAssistant.Core`, `ConferenceAssistant.Ingestion`, `ConferenceAssistant.Agents`, `ConferenceAssistant.Mcp`

### ConferenceAssistant.CopilotDemo *(planned — not yet implemented)*
```xml
<PackageReference Include="GitHub.Copilot.SDK" />
```

### ConferenceAssistant.AppHost
```xml
<PackageReference Include="Aspire.Hosting.Azure.CognitiveServices" />
<PackageReference Include="Aspire.Hosting.DevTunnels" />
<PackageReference Include="Aspire.Hosting.PostgreSQL" />
<PackageReference Include="Aspire.Hosting.Qdrant" />
```
Project reference: `ConferenceAssistant.Web` (as Aspire resource)

The AppHost provisions a PostgreSQL container for relational data, a Qdrant container for vector/embedding storage, and a pgWeb admin container for database inspection:
```csharp
var postgres = builder.AddPostgres("postgres")
    .WithPgWeb()
    .WithDataVolume();

var conferenceDb = postgres.AddDatabase("conferencedb");

var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
```

The web project receives a reference to `conferenceDb` and waits for it to be ready before starting.

#### Dev Tunnel
The AppHost configures a dev tunnel with anonymous access so attendees can reach the web app from their devices:
```csharp
builder.AddDevTunnel("conference-tunnel")
    .WithReference(web)
    .WithAnonymousAccess();
```
The tunnel URL appears in the Aspire dashboard. The Display page's QR code automatically uses the tunnel URL when accessed through it (via `NavigationManager.BaseUri`). Prerequisite: `devtunnel user login` (one-time setup).

### ConferenceAssistant.ServiceDefaults
```xml
<PackageReference Include="Microsoft.Extensions.Http.Resilience" />
<PackageReference Include="Microsoft.Extensions.ServiceDiscovery" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" />
<PackageReference Include="OpenTelemetry.Instrumentation.Runtime" />
```

---

## Data Flow

### Flow 1: Poll Generation
```
Speaker clicks "Generate Poll"
  → Presenter.razor sends event via SessionStateService
    → PollGenerationWorkflow starts
      → SurveyArchitect agent:
          1. Gets current topic from SessionService (may be null)
          2. Calls search_knowledge tool → VectorStore.SearchAsync()
          3. Gets context about what audience already knows
          4. Generates poll question + options via IChatClient
          5. Calls create_poll tool → PollService.CreatePoll()
      → Poll in Draft status (TopicId is optional — session-level polls allowed)
  → Speaker reviews poll in Presenter center column
  → Speaker clicks "Launch"
    → PollService.LaunchPoll() → status = Active
      → SignalR pushes to all circuits
        → Session.razor shows voting UI
        → Display.razor shows live chart
```

> **Note:** `Poll.TopicId` is `string?` — polls can be created without an active topic. Custom polls pass `activeTopic?.Id` (null if no topic is active), enabling session-level polls.

### Flow 2: Response Analysis
```
Speaker clicks "Close Poll & Analyze"
  → PollService.ClosePoll()
    → ResponseIngestionPipeline ingests all responses
      → Chunked, sentiment-enriched, stored in QdrantVectorStore
    → SessionPersistenceService saves poll + responses to PostgreSQL
    → ResponseAnalysisWorkflow starts
      → ResponseAnalyst agent:
          1. Calls get_poll_results tool → tallied results
          2. Searches vector store for audience context
          3. Generates insight via IChatClient
          4. Calls store_insight tool → PostgreSQL
      → KnowledgeCurator agent (handoff):
          1. Searches vector store for related content
          2. Optionally calls Microsoft Learn MCP for docs
          3. Ingests MCP response into vector store
          4. Enriches the insight with doc links
    → Insight pushed via SignalR
      → Display.razor InsightPanel updates
      → Presenter.razor shows detailed view
```

### Flow 3: Audience Q&A
```
Attendee types question in Session.razor
  → SessionStateService.AddQuestion()
    → Question persisted to PostgreSQL via SessionPersistenceService
    → Question ingested into vector store (keyword enriched)
    → KnowledgeCurator agent triggered:
        1. Searches vector store for relevant context
        2. Calls Microsoft Learn MCP if needed
        3. Ingests any new docs
        4. Generates answer via IChatClient
    → Answer attached to question
      → SignalR pushes update
        → QuestionFeed updates on all views
```

> **Global Q&A in Presenter view:** The Presenter right column shows all questions regardless of topic. Each question displays a topic badge indicating which topic it originated from, giving the speaker full visibility across the session.

### Flow 4: Session Summary (The Closer)
```
Copilot SDK console app runs:
  CopilotClient → McpServers["conference-pulse"]
    → "Write a comprehensive summary"
      → Copilot discovers MCP tools
        → Calls generate_session_summary
          → Our MCP server routes to SessionSummaryWorkflow
            → Group chat: SurveyArchitect + ResponseAnalyst + KnowledgeCurator
              → Each searches vector store for their perspective
              → Collaborative summary generated
            → Summary returned as MCP tool result
          → Copilot formats and streams to console
```

### Flow 5: Slide Navigation
```
Speaker clicks "Next" (or presses →/Space)
  → Presenter.razor calls SessionService.AdvanceSlideAsync()
    → _activeSlideIndex incremented
      → SlideChanged event fires
        → SyncTopicToSlide in SessionContext auto-activates topic
          when slide crosses a topic boundary
        → Display.razor receives event via InvokeAsync
          → SlideRenderer re-renders with new slide
        → Presenter.razor updates preview + speaker notes
        → Left column topic outline auto-highlights current topic
```

#### Keyboard Shortcuts (Presenter)

| Key | Action |
|-----|--------|
| `→` / `Space` | Advance slide |
| `←` | Previous slide |
| `P` | Quick-launch poll |
| `Esc` | Close active overlay |

Keyboard shortcuts are suppressed when focus is in a text input (poll question, answer form, etc.).

---

## Data Persistence

The app uses a **hybrid persistence architecture** combining PostgreSQL (relational + vector) for durable storage with in-memory `SessionContext` objects for real-time SignalR event delivery.

### PostgreSQL Infrastructure

The Aspire AppHost provisions PostgreSQL and Qdrant containers:

- **PostgreSQL server** — container for relational data (sessions, polls, Q&A, insights)
- **Data volume** — `WithDataVolume()` persists data across container restarts
- **pgWeb** — `WithPgWeb()` adds a browser-based database admin UI (visible in Aspire dashboard)
- **conferencedb** — named database for all application data
- **Qdrant** — vector database for semantic search, with persistent data volume and `ContainerLifetime.Persistent`

### EF Core — Relational Persistence

`ConferenceDbContext` (`ConferenceAssistant.Web/Data/ConferenceDbContext.cs`) maps 8 entity types to PostgreSQL tables:

| Table | Entity | Key JSONB Columns |
|-------|--------|-------------------|
| `sessions` | ConferenceSession | — |
| `session_topics` | SessionTopic | TalkingPoints, SuggestedPolls |
| `slides` | Slide | Bullets |
| `polls` | Poll | Options |
| `poll_responses` | PollResponse | — |
| `audience_questions` | AudienceQuestion | — |
| `question_answers` | QuestionAnswer | — |
| `insights` | Insight | — |

JSONB columns store complex types (`List<string>`, `List<SuggestedPoll>`) as native PostgreSQL JSON, enabling rich queries without separate join tables. Enum properties (Status, Type, Source) are stored as strings. Shadow FK properties (e.g., `SessionId`, `QuestionId`) establish relationships without polluting domain models.

Schema is auto-created on startup via `EnsureCreatedAsync` — no migrations needed.

### Qdrant — Vector Persistence

`QdrantVectorStore` from `Microsoft.SemanticKernel.Connectors.Qdrant` provides vector storage for semantic search:

- **Collection name**: `conference_knowledge`
- **Record model**: `ConferenceRecord` with 1536-dimensional float vectors
- **Distance metric**: Cosine similarity
- **Qdrant client**: `QdrantClient` injected via Aspire's `Aspire.Qdrant.Client` integration
- **Persistent storage**: Qdrant container uses `WithDataVolume()` + `ContainerLifetime.Persistent`

```csharp
var vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: false, storeOptions);
var collection = vectorStore.GetCollection<string, ConferenceRecord>("conference_knowledge");
```

Vector data survives restarts — the knowledge base does not need to be re-ingested each time the app starts.

### SessionPersistenceService — Save-on-Mutation

`SessionPersistenceService` (`ConferenceAssistant.Web/Services/`) implements durable persistence with event-driven writes:

| Event | What's Saved |
|-------|-------------|
| Session created | Full session + topics + slides |
| Poll created/launched/closed | Poll entity (upsert) |
| Vote cast | PollResponse entity (insert) |
| Question asked | AudienceQuestion entity (upsert) |
| Question answered | QuestionAnswer entity (insert) |
| Insight generated | Insight entity (insert) |

Uses `IDbContextFactory<ConferenceDbContext>` for short-lived DbContext instances (safe for async event handlers).

### Session Restoration

On startup, `LoadAllSessionsAsync()` restores previously saved sessions from PostgreSQL. In-memory `SessionContext` objects are rebuilt for real-time SignalR event delivery. This gives the best of both worlds:

- **Durability** — sessions, polls, questions, and insights survive app restarts
- **Real-time** — Blazor components subscribe to in-memory events for instant UI updates

### Clear Runtime Data

`ClearRuntimeDataAsync(sessionId)` removes polls, poll responses, questions, question answers, and insights from both the database and in-memory state — while preserving the session/topic/slide structure. This enables a "reset for next run" workflow without recreating the session.

---

## SignalR / Real-Time Strategy

Blazor Interactive Server uses SignalR circuits by default. We leverage this:

1. **SessionStateService** — singleton service holding all state. When state changes, it raises events.
2. **Components subscribe** — each Blazor component subscribes to relevant events in `OnInitializedAsync`.
3. **InvokeAsync(StateHasChanged)** — components call this when notified of changes.
4. **No additional SignalR hubs needed** — Blazor Server circuits handle everything.

```csharp
// SessionStateService pattern:
public class SessionStateService
{
    public event Action<Poll>? OnPollCreated;
    public event Action<Poll>? OnPollLaunched;
    public event Action<string, string>? OnVoteReceived;  // pollId, option
    public event Action<Insight>? OnInsightGenerated;
    public event Action<AudienceQuestion>? OnQuestionAsked;
    public event Action<AudienceQuestion>? OnQuestionAnswered;
    public event Action<SessionTopic>? OnTopicChanged;
    public event Action<string>? OnAgentActivity;  // activity description
    public event Action<string>? OnSummaryChunk;   // streaming summary text
    public event Action<int>? OnSlideChanged;      // active slide index
}
```

---

## Slide System

### Markdown-First Approach

Slides are authored in `data/slides.md` using Marp-inspired conventions. The Markdown file is parsed once at startup by `SlideMarkdownParser` (in `ConferenceAssistant.Core/Services/`).

### Parser Behavior

`SlideMarkdownParser` processes the Markdown deck as follows:
1. Splits the file on `---` delimiters (horizontal rules)
2. Extracts `<!-- speaker: ... -->` HTML comments as presenter-only speaker notes
3. Reads `<!-- topic: id -->` comments to map slides to `SessionTopic` entries
4. Auto-detects slide type from content structure (fenced code blocks → Code, `#` only → Title, bullets → Content, etc.)

### Slide Model

```csharp
public class Slide
{
    public SlideType Type { get; set; }    // Title, Content, Code, Section, Blank
    public string? Layout { get; set; }
    public string? Title { get; set; }
    public List<string> Bullets { get; set; }
    public string? CodeSnippet { get; set; }
    public string? SpeakerNotes { get; set; }  // presenter-only
    public string? TopicId { get; set; }
}
```

### Display Priority

The `/display/{SessionCode}` view renders content using this priority order:

1. **Active Poll** — poll voting/results take over the full screen
2. **Active Slide** — the current slide from the deck
3. **Latest Insight** — AI-generated insight panel
4. **QR Code (idle state)** — large "Scan to Join" QR code with session code + URL

When content is active (slide or poll), a smaller QR code appears in the sidebar so latecomers can still join.

The QR code is generated server-side by `QrCodeGenerator.cs`, a static utility using `PngByteQRCode` from the QRCoder NuGet package for cross-platform compatibility (no native image dependencies).

### Speaker Notes

Speaker notes (from `<!-- speaker: ... -->` comments) are **presenter-only**. They appear in the `/presenter/{SessionCode}` dashboard alongside the current slide but are never rendered on `/display/{SessionCode}` or `/session/{SessionCode}`.

---

## GitHub Repository Import Pipeline

The app can ingest content from public GitHub repositories to enrich the knowledge base and auto-generate session structure.

### Pipeline Architecture

```
GitHub Repo URL
  │
  ├─→ ContentImportService (orchestrator)
  │     ├─→ IIngestionService.IngestGitHubRepoAsync
  │     │     ├─→ GitHubRepoReader (fetches .md files via GitHub API)
  │     │     ├─→ MarkdownFrontMatterParser (extracts YAML metadata)
  │     │     ├─→ FrontMatterEnricher (adds metadata to records)
  │     │     └─→ VectorStoreWriter (stores in knowledge base)
  │     │
  │     └─→ ISessionDraftingService.DraftSessionAsync
  │           ├─→ IChatClient (AI generates session structure)
  │           └─→ Fallback: category-grouped topics from front matter
  │
  └─→ SessionDraft (topics, talking points, polls)
        └─→ Presenter reviews → CreateSession
```

### Key Components

| Component | Location | Purpose |
|-----------|----------|---------|
| `GitHubRepoReader` | Ingestion/Readers/ | Fetches .md files from GitHub repos |
| `MarkdownFrontMatterParser` | Ingestion/Utilities/ | Parses YAML front matter without external libs |
| `FrontMatterEnricher` | Ingestion/Enrichers/ | Enriches records with front matter metadata |
| `SessionDraftingService` | Web/Services/ | AI-powered session structure generation |
| `ContentImportService` | Web/Services/ | Orchestrates import → draft pipeline |

### Entry Points

1. **Create Session page** (`/create`) — "🐙 Import from GitHub" template option
2. **Presenter page** (`/presenter/{code}`) — "📥 Import" section (left column) for enriching KB during sessions

### Data Flow

- **Knowledge base enrichment**: Imported markdown → chunked → embedded → vector store → available for Q&A
- **Session drafting**: AI reads front matter + content → generates topics with talking points and polls
- **Dual benefit**: One import action enriches both the knowledge base AND the session structure

### Slide Generation

The system auto-generates slide decks for imported sessions using a two-phase approach:

**Phase 1 — Programmatic Generation** (always runs):
- Deterministically creates slides from `SessionDraft` structure
- Title slide → Section slides per topic → Content slides from talking points → Poll slides → Closing slide
- Follows the same markdown format as `data/slides.md` (`---` separators, `<!-- topic/layout/speaker -->` comments)
- Parsed by `SlideMarkdownParser` into `List<Slide>` objects

**Phase 2 — AI Enhancement** (attempted first, falls back to Phase 1):
- Sends programmatic baseline + imported document content to AI
- AI enriches speaker notes, adds code example slides, improves transitions
- Validated by re-parsing — falls back to Phase 1 if AI output is malformed

| Component | Location | Purpose |
|-----------|----------|---------|
| `SlideGenerationService` | Web/Services/ | Generates slide markdown from SessionDraft |
| `SlideMarkdownParser` | Core/Services/ | Parses slide markdown into Slide objects |
| `SlideRenderer` | Web/Components/Shared/ | Renders slides (Title, Content, Code, Section, Poll, Blank) |

**Topic ID Remapping**: Generated slides use placeholder IDs (`topic-0`, `topic-1`). After session creation, IDs are remapped to match actual topic GUIDs for proper slide-topic linking.

---

## Configuration (appsettings.json)

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://{resource}.openai.azure.com/",
    "ChatDeployment": "gpt-4o",
    "EmbeddingDeployment": "text-embedding-3-small",
    "ApiKey": ""
  },
  "Session": {
    "Code": "AICONF",
    "OutlinePath": "data/session-outline.md",
    "TopicsPath": "data/seed-topics.json"
  },
  "Mcp": {
    "ServerPath": "/mcp",
    "Clients": {
      "MicrosoftLearn": "https://learn.microsoft.com/api/mcp",
      "DeepWiki": "https://deepwiki.com/api/mcp"
    }
  }
}
```

Environment variable overrides:
- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_API_KEY`
- `AZURE_OPENAI_CHAT_DEPLOYMENT`
- `AZURE_OPENAI_EMBEDDING_DEPLOYMENT`
