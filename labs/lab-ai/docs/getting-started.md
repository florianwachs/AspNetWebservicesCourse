# Getting Started with Conference Pulse

Welcome! This guide takes you from zero to a fully running **Conference Pulse** app — an interactive AI-powered conference assistant built with the Microsoft AI stack for .NET. By the end, you'll have the app running locally with Aspire, containers spun up, and three browser tabs showing the presenter, display, and attendee views.

> 💡 **What is Conference Pulse?** It's not a presentation *about* AI — it's a presentation *that is* AI. The app demonstrates five Microsoft AI technologies (Microsoft.Extensions.AI, Data Ingestion, Vector Data, Agent Framework, and MCP) through a live, interactive conference experience.

---

## 1. What You'll Need

Before you begin, make sure you have the following installed:

| Prerequisite | Why You Need It |
|---|---|
| [**.NET 10 SDK**](https://dotnet.microsoft.com/download/dotnet/10.0) | The app targets .NET 10 |
| **Docker Desktop** | Aspire manages PostgreSQL, Qdrant, PgWeb, and the default Ollama container for you |
| [**Aspire CLI**](https://aspire.dev) | The orchestration layer that ties everything together |
| **AI provider** | Use the default local Ollama setup, or configure GitHub Models with a GitHub token |

### Install the Aspire CLI

The Aspire CLI is a standalone tool that orchestrates your app, containers, and cloud resources.

**Windows (PowerShell):**
```powershell
irm https://aspire.dev/install.ps1 | iex
```

**Linux / macOS (Bash):**
```bash
curl -fsSL https://aspire.dev/install.sh | bash
```

Verify the installation:
```bash
aspire --version
```

> ℹ️ For more details, see the [Aspire CLI install guide](https://aspire.dev/get-started/install-cli/). You can also run `aspire doctor` after installation to verify your environment is ready.

### AI provider options

Conference Pulse supports two provider modes:

| Provider | Best for | Default models |
|---|---|---|
| `Ollama` | Local demos without cloud accounts | `llama3.2:3b` for chat, `embeddinggemma` for embeddings |
| `GitHubModels` | Cloud-hosted model access through GitHub | `openai/gpt-4.1-mini` for chat, `openai/text-embedding-3-small` for embeddings |

The repository defaults to `Ollama`. The first run downloads the models into a persistent Docker volume.

---

## 2. Clone and Configure

### Clone the repository

```bash
git clone https://github.com/your-org/dotnet-ai-conference-assistant.git
cd dotnet-ai-conference-assistant
```

### Default: local Ollama

No secrets are needed for the default local Ollama mode. Aspire starts the Ollama container and injects its endpoint into the web app.

### Alternative: GitHub Models

Create a fine-grained GitHub personal access token with `models: read`, then set these secrets from the repository root:

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "AI:Provider" "GitHubModels"
dotnet user-secrets set "AI:ApiKey" "<your-github-token>"
cd ../..
```

---

## 3. Build and Run

### Pre-flight checks

Before launching the app, make sure:

- ✅ **Docker Desktop is running** (Aspire needs it for PostgreSQL, Qdrant, PgWeb, and local Ollama)
- ✅ **GitHub Models only:** `AI:ApiKey` is set to a token with `models: read`

### Launch the app

```bash
# Build the solution (optional — Aspire builds automatically on run)
dotnet build

# Run with Aspire
aspire run
```

> ℹ️ Don't have the Aspire CLI? See [Install the Aspire CLI](#install-the-aspire-cli) above, or run `aspire doctor` to diagnose setup issues.

### What happens when you run

When `aspire run` starts, several things happen automatically:

1. **PostgreSQL container** spins up — stores sessions, polls, Q&A, and insights
2. **Qdrant container** spins up — vector database for semantic search
3. **PgWeb container** spins up — a web-based UI for inspecting the PostgreSQL database
4. **AI provider starts/connects** — Ollama downloads local models, or GitHub Models is configured from your token
5. **The web project** builds, starts, and connects to all resources
6. **A dev tunnel** is created — provides a public HTTPS URL so attendees can join from their phones
7. **Default demo data** loads — a session called `DOTNETAI-CONF` is auto-created with five pre-configured topics
8. **Slides and knowledge base** are ingested — content from `data/slides.md` is parsed and indexed

> ⚠️ **First run takes longer.** Docker needs to pull the PostgreSQL and Qdrant images. In Ollama mode it also downloads `llama3.2:3b` and `embeddinggemma`; keep Aspire running until the model resources are healthy.

---

## 4. The Aspire Dashboard

Once the app is running, Aspire opens its dashboard in your browser automatically. This is your command center for the entire distributed app.

### What to look for

- **All resources should show "Running" status** — you should see entries for `web`, `postgres`, `qdrant`, `pgweb`, plus either `ollama` model resources or GitHub Models resources
- **The web app URL** — find the `web` resource and click its endpoint link. This is the URL you'll open in your browser.
- **Logs and traces** — click any resource to see its logs, structured traces, and health status

> ⚠️ **Ports are dynamic!** Aspire assigns ports at runtime, so they change between runs. Always check the dashboard for the actual URLs — don't bookmark specific port numbers.

### Finding the web app URL

In the dashboard, look for the resource named **`web`**. Its endpoint column shows the local URL (something like `https://localhost:7xxx`). Click it to open the app.

If a dev tunnel is configured, you'll also see a public HTTPS URL — this is what you share with attendees so they can join from their phones.

---

## 5. Your First Exploration

With the app running, open **three browser tabs** to see Conference Pulse from every angle. Use the URL from the Aspire dashboard as your base URL.

### 🏠 Home Page — `/`

The landing page lists all active sessions. You'll see the default **"DOTNETAI-CONF"** session already created. From here you can also create new sessions with custom topics and configurations.

### 🎙️ Presenter Dashboard — `/presenter/DOTNETAI-CONF`

This is the speaker's command center — what you'd have open on your laptop during a talk. When prompted for the PIN, enter **`0000`** (the default).

From here you can:
- Navigate between topics
- Launch and close polls
- Review audience questions
- Trigger AI-generated insights
- Control the display view

> 💡 **Try it:** Open this view and explore the five pre-configured topics. Each one comes with talking points and suggested polls ready to go.

### 📺 Display View — `/display/DOTNETAI-CONF`

This is what goes on the big screen — the projector or external monitor that the audience sees. It shows:
- Active polls with real-time vote counts
- AI-generated insights and analysis
- Audience questions and answers
- Topic transitions and session status

### 📱 Attendee Session — `/session/DOTNETAI-CONF`

This is what audience members see on their phones. They can:
- Vote on active polls
- Submit questions
- See real-time results and insights

> 💡 **Simulate an audience:** Open this URL in a few browser tabs (or on your phone) to see how the attendee experience works. Submit some poll votes and questions, then watch them appear in the presenter dashboard and display view.

---

## 6. What's Next?

Now that you have Conference Pulse running, here's where to go from here:

### Tutorials

- **[Your First Session](tutorials/01-your-first-session.md)** — Walk through a complete session lifecycle: create a session, run polls, answer questions, and generate a summary
- **[Running a Live Demo](tutorials/02-running-a-live-demo.md)** — Practice the full demo flow so you're confident presenting in front of an audience
- **[Customize for Your Talk](tutorials/03-customize-for-your-talk.md)** — Adapt the topics, polls, and content for your own presentation

### Reference

- **[Technology Guide](technology-guide.md)** — Deep dive into how each Microsoft AI technology (Extensions.AI, Data Ingestion, Vector Data, Agent Framework, MCP) is used in the app
- **[Configuration Reference](configuration.md)** — All configuration options, environment variables, and settings
- **[Troubleshooting](troubleshooting.md)** — Common issues and solutions for setup, containers, and provider connectivity

---

> 💡 **Tip:** If something isn't working, check the [Troubleshooting](troubleshooting.md) guide first. Most local issues come down to Docker not running or Ollama model downloads still being in progress; GitHub Models issues usually come down to a missing or under-scoped token.
