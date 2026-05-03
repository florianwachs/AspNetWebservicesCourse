# Getting Started with Conference Pulse

Welcome! This guide takes you from zero to a fully running **Conference Pulse** app — an interactive AI-powered conference assistant built with the Microsoft AI stack for .NET. By the end, you'll have the app running locally with Aspire, containers spun up, and three browser tabs showing the presenter, display, and attendee views.

> 💡 **What is Conference Pulse?** It's not a presentation *about* AI — it's a presentation *that is* AI. The app demonstrates five Microsoft AI technologies (Microsoft.Extensions.AI, Data Ingestion, Vector Data, Agent Framework, and MCP) through a live, interactive conference experience.

---

## 1. What You'll Need

Before you begin, make sure you have the following installed:

| Prerequisite | Why You Need It |
|---|---|
| [**.NET 10 SDK**](https://dotnet.microsoft.com/download/dotnet/10.0) | The app targets .NET 10 |
| **Docker Desktop** | Aspire manages PostgreSQL, Qdrant, and PgWeb containers for you |
| [**Aspire CLI**](https://aspire.dev) | The orchestration layer that ties everything together |
| [**Azure CLI**](https://learn.microsoft.com/cli/azure/install-azure-cli) | Authentication via `az login` — no API keys needed |
| **Azure OpenAI resource** | Powers chat completions, agent reasoning, and semantic search |

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

### Azure OpenAI deployments

Your Azure OpenAI resource needs exactly **two model deployments**:

| Deployment Name | Model | Model Version |
|---|---|---|
| `chat` | `gpt-4o` | `2024-08-06` |
| `embedding` | `text-embedding-3-small` | `1` |

> ⚠️ **The deployment names must be exactly `chat` and `embedding`.** The AppHost references them by these names — using different names will cause connection failures.

---

## 2. Setting Up Azure OpenAI

If you don't have an Azure OpenAI resource yet, here's how to create one.

### Create the resource

1. Go to the [Azure Portal](https://portal.azure.com)
2. Search for **"Azure OpenAI"** and select **Create**
3. Choose your subscription, resource group, and region
4. Give it a name (you'll need this later)
5. Select the **Standard S0** pricing tier
6. Complete the wizard and wait for deployment

> 📖 For detailed instructions, see [Create and deploy an Azure OpenAI Service resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource) on Microsoft Learn.

### Create the model deployments

1. Open your Azure OpenAI resource in the Azure Portal
2. Go to **Model deployments** → **Manage Deployments** (this opens Azure AI Foundry)
3. Create two deployments:

**First deployment:**
- Name: **`chat`**
- Model: **gpt-4o**
- Model version: **2024-08-06**

**Second deployment:**
- Name: **`embedding`**
- Model: **text-embedding-3-small**
- Model version: **1**

> 💡 **No API keys required!** Conference Pulse uses `DefaultAzureCredential`, which means it picks up your identity from `az login` automatically. No need to copy and paste keys.

---

## 3. Clone and Configure

### Clone the repository

```bash
git clone https://github.com/your-org/dotnet-ai-conference-assistant.git
cd dotnet-ai-conference-assistant
```

### Set user secrets

The AppHost project needs four user secrets to connect to your Azure OpenAI resource. Run these commands from the repository root:

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "Azure:SubscriptionId" "<your-subscription-id>"
dotnet user-secrets set "Azure:Location" "eastus"
dotnet user-secrets set "AzureOpenAI:Name" "<your-openai-resource-name>"
dotnet user-secrets set "AzureOpenAI:ResourceGroup" "<your-resource-group-name>"
cd ../..
```

Replace the placeholder values with your actual Azure details.

### What each secret does

| Secret | Description | Example |
|---|---|---|
| `Azure:SubscriptionId` | Your Azure subscription ID — Aspire uses this for local Azure resource provisioning | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `Azure:Location` | Azure region where your resources are deployed | `eastus`, `westus2`, `northeurope` |
| `AzureOpenAI:Name` | The name of your Azure OpenAI resource (as shown in the Azure Portal) | `my-openai-resource` |
| `AzureOpenAI:ResourceGroup` | The resource group containing your Azure OpenAI resource | `my-rg` |

> 💡 **Where to find your subscription ID:** Run `az account show --query id -o tsv` in your terminal, or find it in the Azure Portal under **Subscriptions**.

---

## 4. Build and Run

### Pre-flight checks

Before launching the app, make sure:

- ✅ **Docker Desktop is running** (Aspire needs it for PostgreSQL, Qdrant, and PgWeb containers)
- ✅ **You're logged in to Azure** (`az login`)

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
4. **The web project** builds, starts, and connects to all resources
5. **A dev tunnel** is created — provides a public HTTPS URL so attendees can join from their phones
6. **Default demo data** loads — a session called `DOTNETAI-CONF` is auto-created with five pre-configured topics
7. **Slides and knowledge base** are ingested — content from `data/slides.md` is parsed and indexed

> ⚠️ **First run takes longer.** Docker needs to pull the PostgreSQL and Qdrant images, and NuGet packages need to restore. Subsequent runs are much faster.

---

## 5. The Aspire Dashboard

Once the app is running, Aspire opens its dashboard in your browser automatically. This is your command center for the entire distributed app.

### What to look for

- **All resources should show "Running" status** — you should see entries for `web`, `postgres`, `qdrant`, `pgweb`, and the Azure OpenAI connection
- **The web app URL** — find the `web` resource and click its endpoint link. This is the URL you'll open in your browser.
- **Logs and traces** — click any resource to see its logs, structured traces, and health status

> ⚠️ **Ports are dynamic!** Aspire assigns ports at runtime, so they change between runs. Always check the dashboard for the actual URLs — don't bookmark specific port numbers.

### Finding the web app URL

In the dashboard, look for the resource named **`web`**. Its endpoint column shows the local URL (something like `https://localhost:7xxx`). Click it to open the app.

If a dev tunnel is configured, you'll also see a public HTTPS URL — this is what you share with attendees so they can join from their phones.

---

## 6. Your First Exploration

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

## 7. What's Next?

Now that you have Conference Pulse running, here's where to go from here:

### Tutorials

- **[Your First Session](tutorials/01-your-first-session.md)** — Walk through a complete session lifecycle: create a session, run polls, answer questions, and generate a summary
- **[Running a Live Demo](tutorials/02-running-a-live-demo.md)** — Practice the full demo flow so you're confident presenting in front of an audience
- **[Customize for Your Talk](tutorials/03-customize-for-your-talk.md)** — Adapt the topics, polls, and content for your own presentation

### Reference

- **[Technology Guide](technology-guide.md)** — Deep dive into how each Microsoft AI technology (Extensions.AI, Data Ingestion, Vector Data, Agent Framework, MCP) is used in the app
- **[Configuration Reference](configuration.md)** — All configuration options, environment variables, and settings
- **[Troubleshooting](troubleshooting.md)** — Common issues and solutions for setup, containers, and Azure connectivity

---

> 💡 **Tip:** If something isn't working, check the [Troubleshooting](troubleshooting.md) guide first. Most issues come down to Docker not running, Azure CLI not being logged in, or deployment names not matching exactly.
