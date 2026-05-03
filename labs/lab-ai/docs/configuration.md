# Configuration Reference

## Overview

Conference Pulse is configured through .NET Aspire orchestration. It supports exactly two AI providers:

- `Ollama` — default local mode. Aspire starts an Ollama Docker container and downloads small local models.
- `GitHubModels` — cloud-hosted GitHub Models using a GitHub token with `models: read` permission.

No API keys are stored in code. Use user secrets or environment variables for provider secrets.

## User Secrets (AppHost Project)

All secrets are set in the AppHost project (`src/ConferenceAssistant.AppHost`):

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "KEY" "VALUE"
```

| Secret | Required | Description | Example |
|--------|----------|-------------|---------|
| `AI:Provider` | Optional | `Ollama` or `GitHubModels`. Defaults to `Ollama` in appsettings. | `GitHubModels` |
| `AI:ApiKey` | GitHub Models only | GitHub fine-grained PAT with `models: read`. | `github_pat_...` |
| `AI:ChatModel` | Optional | Chat model override. | `openai/gpt-4.1-mini` |
| `AI:EmbeddingModel` | Optional | Embedding model override. | `openai/text-embedding-3-small` |
| `AI:EmbeddingDimensions` | Optional | Vector size used by Qdrant. | `1536` |
| `AI:VectorCollectionName` | Optional | Qdrant collection to use for this provider/model pair. | `conference_knowledge_github_models` |

## AI Provider Defaults

| Provider | Endpoint | Chat Model | Embedding Model | Dimensions | Collection |
|----------|----------|------------|-----------------|------------|------------|
| `Ollama` | `http://localhost:11434/v1` outside Aspire; Aspire injects its container endpoint when orchestrated | `llama3.2:3b` | `embeddinggemma` | `768` | `conference_knowledge_ollama` |
| `GitHubModels` | `https://models.github.ai/inference` | `openai/gpt-4.1-mini` | `openai/text-embedding-3-small` | `1536` | `conference_knowledge_github_models` |

When switching providers, keep separate collection names unless you intentionally rebuild the vector data. Qdrant collections have fixed vector dimensions, so old Azure/OpenAI 1536-dimensional data cannot be searched with Ollama's 768-dimensional embeddings.

## Infrastructure (Aspire-Managed)

These are configured in the AppHost and managed automatically by Aspire:

| Resource | Type | Purpose | Configuration |
|----------|------|---------|--------------|
| PostgreSQL | Container | EF Core persistence (sessions, polls, Q&A, insights) | Data volume for persistence across restarts. PgWeb admin UI available via `.WithPgWeb()`. |
| Qdrant | Container | Vector/embedding storage for semantic search | Persistent lifetime (`.WithLifetime(ContainerLifetime.Persistent)`), data volume. |
| Ollama | Container | Local chat and embedding models when `AI:Provider` is `Ollama` | Persistent model volume. First run downloads `llama3.2:3b` and `embeddinggemma`. |
| Dev Tunnel | Azure Dev Tunnel | Public HTTPS URL for attendee access | Anonymous access enabled via `.WithAnonymousAccess()`. |

## MCP Server Configuration

The MCP server (`ConferencePulse` v1.0.0) is exposed at `/mcp` on the web app. To connect VS Code or Copilot CLI:

`.vscode/mcp.json`:
```jsonc
{
  "servers": {
    "ConferencePulse": {
      "type": "http",
      "url": "http://localhost:{PORT}/mcp"
    }
  }
}
```

> ⚠️ Port is dynamic — check the Aspire dashboard for the actual web endpoint.

## Default Demo Session

At startup, the app auto-creates a demo session from `data/seed-topics.json`:

- **Session Code:** `DOTNETAI-CONF`
- **PIN:** `0000` (for presenter dashboard access)
- **Topics:** 5 pre-configured topics (Microsoft.Extensions.AI, Knowledge Engineering, Agentic AI, Interoperability with MCP, The Closer)
- **Slides:** Loaded from `data/slides.md`

To customize, edit:

- `data/seed-topics.json` — Topics, descriptions, suggested polls
- `data/slides.md` — Presentation slides
- `data/session-outline.md` — Content ingested into the knowledge base at startup

## AI Middleware Pipeline

The `IChatClient` is configured with middleware in `src/ConferenceAssistant.Web/Program.cs`:

```csharp
builder.Services.AddChatClient(...)
    .UseFunctionInvocation()   // Enables agent tool calling
    .UseOpenTelemetry()        // Distributed tracing
    .UseLogging();             // Request/response logging
```

## Running Modes

### Ollama

Ollama is the default:

```bash
aspire run
```

Keep the Aspire dashboard open until both Ollama model resources are healthy. The first start downloads the models.

### GitHub Models

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "AI:Provider" "GitHubModels"
dotnet user-secrets set "AI:ApiKey" "<github-token-with-models-read>"
cd ../..
aspire run
```

## Environment Variables

Aspire injects connection strings automatically. The web project receives:

- `ConnectionStrings__conferencedb` — PostgreSQL connection string
- `ConnectionStrings__qdrant` — Qdrant endpoint
- `AI__Provider`, `AI__Endpoint`, `AI__ApiKey`, `AI__ChatModel`, `AI__EmbeddingModel`, `AI__EmbeddingDimensions`, `AI__VectorCollectionName` — AI provider settings from AppHost

These are handled by Aspire and should not be set manually.
