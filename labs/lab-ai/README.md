# Conference Pulse — The Living Presentation

I modified the sample from https://github.com/luisquintanilla/dotnet-ai-conference-assistant for our course.
Thanks for providing it!

> Not a presentation *about* AI — a presentation *that is* AI.

An interactive .NET conference assistant that runs as a live web app during your talk. The presentation surface itself is powered by the Microsoft AI stack — agents generate polls, analyze audience responses, build a growing knowledge base, and produce real-time insights. Each segment makes the app smarter (the "snowball effect").

## The Six Technologies

| # | Technology | Role | Status |
|---|-----------|------|--------|
| 1 | **Microsoft.Extensions.AI** | The abstraction layer — `IChatClient`, `IEmbeddingGenerator`, `ChatClientBuilder` middleware | ✅ Implemented |
| 2 | **Microsoft.Extensions.DataIngestion** | Feeding the brain — markdown → chunks → enrichment → vector store | ✅ Implemented |
| 3 | **Microsoft.Extensions.VectorData** | The memory — Qdrant vector store with semantic search | ✅ Implemented |
| 4 | **Microsoft Agent Framework** | The intelligence — specialized agents with tools and workflows | ✅ Implemented |
| 5 | **Model Context Protocol (MCP)** | The bridge — our app as both MCP server and client | ✅ Implemented |
| 6 | **Microsoft Copilot SDK** | The closer — Copilot as a first-class consumer of our MCP tools | 🔜 Planned |

**The Closer:** Speaker opens Copilot CLI → points it at our MCP server → types "Summarize this session" → cascade visualization lights up on the big screen as all technologies fire in sequence.

## Two Ways to Use This Project

- **📚 As a Reference Project** — Learn the Microsoft AI stack for .NET end-to-end. Each project in the solution isolates one technology so you can study them independently. Start with the [architecture guide](docs/architecture.md) and [implementation spec](docs/implementation-spec.md).
- **🎤 As a Presentation Tool** — Run it live during your own conference talk. Fork the repo, customize `data/slides.md` and `data/seed-topics.json` for your content, and present. See the [slide authoring guide](docs/slide-authoring-guide.md) and [session outline](docs/session-outline.md) for the demo flow.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Aspire CLI](https://aspire.dev) — install via `irm https://aspire.dev/install.ps1 | iex` (Windows) or `curl -fsSL https://aspire.dev/install.sh | bash` (Linux/macOS)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL, Qdrant, and optional Ollama containers)
- One AI provider:
  - **Ollama** (default) — Aspire runs `qwen2.5:0.5b-instruct` and `nomic-embed-text` in Docker.
  - **GitHub Models** — requires a GitHub token with `models: read` permission.

## Quick Start

```bash
# Clone
git clone https://github.com/your-org/dotnet-ai-conference-assistant.git
cd dotnet-ai-conference-assistant

# Run with Aspire (default: Ollama in Docker)
aspire run
```

To use GitHub Models instead:

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "AI:Provider" "GitHubModels"
dotnet user-secrets set "AI:ApiKey" "github_pat_with_models_read"
cd ../..
aspire run
```

> **Ports are dynamic** — open the Aspire dashboard to find the web app URL.

Once running, open these views:

| Route | Purpose |
|-------|---------|
| `/` | Home — lists active sessions, create a new one |
| `/create` | Create a new session (template or blank) |
| `/presenter/{SessionCode}` | Speaker dashboard (your laptop) |
| `/display/{SessionCode}` | Projection view (big screen) |
| `/session/{SessionCode}` | Attendee participation (audience phones via QR code) |

## Architecture

```
┌──────────────────────────────────────────────────────────┐
│                    .NET Aspire AppHost                    │
│  ┌────────────┐  ┌────────────┐  ┌────────────────────┐  │
│  │ PostgreSQL │  │   Qdrant   │  │   AI Provider      │  │
│  │  + PgWeb   │  │  (vector)  │  │ GitHub or Ollama   │  │
│  │  + volume  │  │  + volume  │  │                    │  │
│  └─────┬──────┘  └─────┬──────┘  └────────┬───────────┘  │
│        └───────────┐   │   ┌──────────────┘              │
│                    ▼   ▼   ▼                             │
│  ┌─────────────────────────────────────────────────────┐ │
│  │  ConferenceAssistant.Web — Blazor Interactive Server │ │
│  │  /presenter  /display  /session  /create            │ │
│  │                                                     │ │
│  │  ┌─ Agent Framework ────────────────────────────┐   │ │
│  │  │  SurveyArchitect · ResponseAnalyst           │   │ │
│  │  │  KnowledgeCurator · Workflows                │   │ │
│  │  └──────────────────────────────────────────────┘   │ │
│  │  ┌─ M.E.AI ────────────────────────────────────┐   │ │
│  │  │  IChatClient → ChatClientBuilder pipeline   │   │ │
│  │  │  IEmbeddingGenerator · AIFunctionFactory    │   │ │
│  │  └──────────────────────────────────────────────┘   │ │
│  │  ┌─ DataIngestion ─────────────────────────────┐   │ │
│  │  │  Outline · Responses · Insights · MCP docs  │   │ │
│  │  └──────────────────────────────────────────────┘   │ │
│  │  ┌─ VectorData ────────────────────────────────┐   │ │
│  │  │  Qdrant vector store + semantic search      │   │ │
│  │  └──────────────────────────────────────────────┘   │ │
│  │  ┌─ MCP ───────────────────────────────────────┐   │ │
│  │  │  SERVER: 10 tools at /mcp                   │   │ │
│  │  │  CLIENT: External knowledge sources         │   │ │
│  │  └──────────────────────────────────────────────┘   │ │
│  └─────────────────────────────────────────────────────┘ │
│        │                                                 │
│  ┌─────▼───────────────────────┐                         │
│  │  Dev Tunnel (public HTTPS)  │ ← attendee phones       │
│  └─────────────────────────────┘                         │
└──────────────────────────────────────────────────────────┘
```

## Project Structure

```
src/
├── ConferenceAssistant.Web/             # Blazor Server + EF Core + Program.cs DI wiring
├── ConferenceAssistant.Core/            # Domain models + in-memory services (no external deps)
├── ConferenceAssistant.Agents/          # Agent tools, definitions, workflows (Microsoft.Agents.AI)
├── ConferenceAssistant.Ingestion/       # DataIngestion pipelines + Qdrant vector search
├── ConferenceAssistant.Mcp/             # MCP server tools + external MCP clients
├── ConferenceAssistant.AppHost/         # .NET Aspire orchestration
└── ConferenceAssistant.ServiceDefaults/ # Aspire shared service defaults + OpenTelemetry
data/
├── seed-topics.json                     # Default session template (topics + polls)
├── session-outline.md                   # Session content (ingested into knowledge base)
└── slides.md                            # Markdown slide deck (display + knowledge base)
docs/
├── architecture.md                      # Technical architecture
├── implementation-spec.md               # Implementation specification
├── mcp-reference.md                     # MCP tool reference
├── prd.md                               # Product requirements
├── session-outline.md                   # Presentation flow
├── slide-authoring-guide.md             # Slide authoring reference
└── smoke-test.md                        # Setup & testing guide
```

## MCP Server

The app exposes an MCP server at `/mcp` with **10 tools** across two tool classes:

### Conference Tools

| Tool | Description |
|------|-------------|
| `get_session_status` | Session title, status, active topic, and all topic statuses |
| `get_active_poll` | Current poll with question, options, and live vote counts |
| `get_poll_results` | Detailed results for a specific poll (by ID) |
| `search_session_knowledge` | Semantic search over the session knowledge base |
| `get_audience_questions` | Top audience questions ordered by upvotes |
| `get_topic_insights` | AI-generated insights for a specific topic |
| `get_all_insights` | All generated insights grouped by type |
| `generate_session_summary` | Comprehensive session summary — polls, insights, Q&A, stats |

### Knowledge Tools

| Tool | Description |
|------|-------------|
| `search_knowledge` | Search the knowledge base by query (returns top 5 matches) |
| `get_knowledge_stats` | Knowledge base record count statistics |

### Copilot CLI Integration

```jsonc
// .vscode/mcp.json
{
  "servers": {
    "ConferencePulse": {
      "type": "http",
      "url": "https://localhost:7231/mcp"
    }
  }
}
```

> ⚠️ Port may vary — check the Aspire dashboard for the actual web endpoint.

Then: `"Summarize this session including all poll results and key themes"`

## Configuration

All configuration flows through **Aspire + user secrets** — no API keys in code.

| Setting | Default | Description |
|---------|---------|-------------|
| `AI:Provider` | `Ollama` | `Ollama` or `GitHubModels` |
| `AI:ChatModel` | `qwen2.5:0.5b-instruct` for Ollama, `openai/gpt-4.1-mini` for GitHub Models | Chat completions model |
| `AI:EmbeddingModel` | `nomic-embed-text` for Ollama, `openai/text-embedding-3-small` for GitHub Models | Embedding model |
| `AI:EmbeddingDimensions` | `768` for Ollama, `1536` for GitHub Models | Qdrant vector size |
| `AI:VectorCollectionName` | provider-specific | Qdrant collection name |
| `AI:ApiKey` | `ollama` for Ollama | Required for GitHub Models |

The default local path uses Ollama in Docker. The first run downloads `qwen2.5:0.5b-instruct` and `nomic-embed-text`, so keep Aspire running until the model downloads complete. Switching providers uses separate Qdrant collections because embedding dimensions differ; existing Azure-era vector data is not reused automatically.

## The Snowball Effect

Each segment enriches the vector store. Agents in later segments have richer context:

```
Segment 1: outline only        → basic poll
Segment 2: + poll responses     → contextual poll
Segment 3: + insights           → trend-aware poll
Segment 4: + MCP docs           → maximum context
Closer:    FULL knowledge base  → comprehensive summary
```

## Slide System

Presentation content is authored in Markdown (`data/slides.md`) using simple conventions:
- `---` separates slides
- `<!-- speaker: notes -->` adds presenter-only speaker notes
- `<!-- topic: id -->` maps slides to session topics

The big screen (`/display/{SessionCode}`) shows slides full-screen between polls. The presenter view (`/presenter/{SessionCode}`) shows the current slide, speaker notes, and a preview of the next slide — like PowerPoint Presenter View.

See [docs/slide-authoring-guide.md](docs/slide-authoring-guide.md) for the full authoring reference.

## Documentation

| Document | Description |
|----------|-------------|
| [Architecture](docs/architecture.md) | System design, component interactions, data flow |
| [Implementation Spec](docs/implementation-spec.md) | Technical implementation details |
| [MCP Reference](docs/mcp-reference.md) | MCP tool documentation and usage |
| [Slide Authoring Guide](docs/slide-authoring-guide.md) | How to write and customize slides |
| [Session Outline](docs/session-outline.md) | Demo presentation flow and talking points |
| [Smoke Test](docs/smoke-test.md) | Setup verification checklist |
| [PRD](docs/prd.md) | Product requirements |

## License

[MIT](LICENSE)
