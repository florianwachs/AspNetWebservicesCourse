# Configuration Reference

## Overview

Conference Pulse is configured through .NET Aspire orchestration — no API keys in code or config files. Authentication uses `DefaultAzureCredential` (Azure CLI, managed identity, etc.).

## User Secrets (AppHost Project)

All secrets are set in the AppHost project (`src/ConferenceAssistant.AppHost`):

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "KEY" "VALUE"
```

| Secret | Required | Description | Example |
|--------|----------|-------------|---------|
| `Azure:SubscriptionId` | Yes | Azure subscription ID for Aspire local provisioning | `12345678-abcd-...` |
| `Azure:Location` | Yes | Azure region for provisioned resources | `eastus` |
| `AzureOpenAI:Name` | Yes | Name of your Azure OpenAI resource | `my-openai-resource` |
| `AzureOpenAI:ResourceGroup` | Yes | Resource group containing the resource | `my-rg` |

## Azure OpenAI Deployments

The AppHost references two deployments by exact name (lines 18–19 in `src/ConferenceAssistant.AppHost/AppHost.cs`):

```csharp
openai.AddDeployment("chat", "gpt-4o", "2024-08-06");
openai.AddDeployment("embedding", "text-embedding-3-small", "1");
```

| Deployment Name | Model | Version | Purpose |
|----------------|-------|---------|---------|
| `chat` | gpt-4o | 2024-08-06 | All chat completions — agent workflows, poll generation, Q&A |
| `embedding` | text-embedding-3-small | 1 | Vector embeddings for ingestion and semantic search |

> ⚠️ Deployment names must match exactly. If your Azure OpenAI resource uses different deployment names, update lines 18–19 in `src/ConferenceAssistant.AppHost/AppHost.cs`.

## Infrastructure (Aspire-Managed)

These are configured in the AppHost and managed automatically by Aspire:

| Resource | Type | Purpose | Configuration |
|----------|------|---------|--------------|
| PostgreSQL | Container | EF Core persistence (sessions, polls, Q&A, insights) | Data volume for persistence across restarts. PgWeb admin UI available via `.WithPgWeb()`. |
| Qdrant | Container | Vector/embedding storage for semantic search | Persistent lifetime (`.WithLifetime(ContainerLifetime.Persistent)`), data volume. |
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
openaiBuilder.AddChatClient("chat")
    .UseFunctionInvocation()   // Enables agent tool calling
    .UseOpenTelemetry()        // Distributed tracing
    .UseLogging();             // Request/response logging
```

## Running Without AI

The app can run without Azure OpenAI configured. Core features (session management, manual polls, voting, Q&A submission) work. AI features (auto-generated polls, AI answers, insights, semantic search) are disabled. See the Aspire dashboard — the OpenAI resource will show as unhealthy.

## Environment Variables

Aspire injects connection strings automatically. The web project receives:

- `ConnectionStrings__openai` — Azure OpenAI endpoint
- `ConnectionStrings__conferencedb` — PostgreSQL connection string
- `ConnectionStrings__qdrant` — Qdrant endpoint

These are handled by Aspire and should not be set manually.
