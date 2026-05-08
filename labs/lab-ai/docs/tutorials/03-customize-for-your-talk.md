# Customize for Your Talk

> **Prerequisites:** This tutorial assumes you've completed [Your First Session](01-your-first-session.md) and understand the basic workflow.

## What You'll Learn

How to customize Conference Pulse for your own conference talk — changing topics, slides, seed data, and presentation content. By the end of this guide, you'll have a fully personalized session ready to run.

---

## Understanding the Content Model

Three files in the `data/` directory control the session content:

| File | Purpose | When Loaded |
|------|---------|-------------|
| `data/seed-topics.json` | Topics, descriptions, and suggested polls for the default session | At startup |
| `data/slides.md` | Markdown slide deck shown on the display view | At startup |
| `data/session-outline.md` | Content ingested into the knowledge base for AI context | At startup |

All three files are loaded when the app starts. Edit them, restart the app, and your customizations are live.

---

## Step 1: Customize Topics (`data/seed-topics.json`)

This file defines the session metadata and every topic your audience will see.

### Root Structure

```json
{
  "sessionId": "YOUR-SESSION-ID",
  "title": "Your Talk Title",
  "description": "A one-line description of your talk.",
  "topics": [ ]
}
```

### Topic Fields

Each object in the `topics` array has these fields:

| Field | Type | Required | Purpose |
|-------|------|----------|---------|
| `id` | string | ✅ | Unique identifier — used in `<!-- topic: id -->` slide directives |
| `title` | string | ✅ | Display title shown on the presenter dashboard and audience view |
| `description` | string | ✅ | Full description visible on the presenter dashboard |
| `order` | number | ✅ | Display order (1, 2, 3…) — controls topic sequence |
| `talkingPoints` | string[] | Optional | Bullet points for the presenter — shown in the dashboard as reminders |
| `suggestedPolls` | object[] | Optional | Pre-written poll questions with answer options |

Each `suggestedPolls` entry has:

| Field | Type | Purpose |
|-------|------|---------|
| `question` | string | The poll question shown to attendees |
| `options` | string[] | Answer choices (typically 3–5 options) |

### Minimal Example

Here's a single topic for a hypothetical microservices talk:

```json
{
  "id": "microservices",
  "title": "Microservices Architecture",
  "description": "Patterns and practices for building microservices with .NET",
  "order": 1,
  "talkingPoints": [
    "Service boundaries and domain-driven design",
    "Communication patterns (sync vs async)",
    "Data ownership per service"
  ],
  "suggestedPolls": [
    {
      "question": "How many microservices does your team maintain?",
      "options": ["1-5", "6-15", "16-50", "50+", "We use a monolith"]
    }
  ]
}
```

### Tips for Good Topics

- **3–5 topics is the sweet spot.** Each should cover 10–20 minutes of your talk.
- **Topic IDs are your glue.** They connect topics to slides via `<!-- topic: id -->` — keep them short and lowercase (e.g., `"containers"`, `"observability"`).
- **Write 2–3 suggested polls per topic.** The AI can also generate polls dynamically, but having suggestions ensures quality and saves time on stage.

---

## Step 2: Customize Slides (`data/slides.md`)

The slide deck is a single Markdown file. Each slide is separated by `---` on its own line.

### Key Conventions

| Syntax | Purpose | Scope |
|--------|---------|-------|
| `---` | Separates slides | Between slides |
| `<!-- topic: id -->` | Maps slides to a topic — **sticky** (applies to all subsequent slides until the next `<!-- topic: -->` directive) | Sticky |
| `<!-- speaker: notes -->` | Presenter-only notes (hidden from the display view) | Single slide |
| `<!-- layout: centered -->` | Centers content (use for title and section slides) | Single slide |

For the full reference — including slide types, layout options, and advanced formatting — see the [Slide Authoring Guide](../slide-authoring-guide.md).

### Slide Types

The parser auto-classifies each slide based on its content:

| Type | Content Pattern | Use For |
|------|----------------|---------|
| **Title** | `# Heading` + `## Subtitle` (nothing else) | Opening and closing slides |
| **Section** | A heading alone (no body content) | Visual dividers between topics |
| **Content** | Heading + bullet points or body text | Main teaching slides |
| **Code** | Heading + fenced code block | Code examples |
| **Blank** | Only `<!-- speaker: -->` notes (no visible content) | Demo pauses, live coding breaks |

### Minimal 3-Slide Example

Here's a starter deck for a "Cloud Native .NET" talk:

```markdown
<!-- layout: centered -->

# Cloud Native .NET

## Building for the Cloud

<!-- speaker:
Welcome! This talk covers containers, orchestration, and observability
for .NET applications.
-->

---

<!-- topic: containers -->

## Why Containers?

- Consistent environments from dev to prod
- Lightweight isolation without full VMs
- First-class support in .NET 8+ with `dotnet publish`
- Built-in health checks and graceful shutdown

<!-- speaker: Ask the audience who's already running containers in production. -->

---

## Container Build Options

```dockerfile
# .NET SDK publish (no Dockerfile needed!)
dotnet publish -t:PublishContainer

# Or use a traditional Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

<!-- speaker: Show both approaches. Emphasize that .NET 8+ can build containers without a Dockerfile. -->
```

### Important Rules

- **Topic IDs must match `data/seed-topics.json`.** If your topic has `"id": "containers"`, your slide directive must be `<!-- topic: containers -->`.
- **Slide content feeds the knowledge base.** Write clear, descriptive bullets — the AI agents use slide text to generate polls and answer questions.
- **Keep slides scannable.** Aim for 5–6 bullets per content slide and 5–10 lines per code slide.

---

## Step 3: Customize the Knowledge Base (`data/session-outline.md`)

This file provides the AI agents with deep context about your talk. It's ingested into the vector store at startup, where it powers poll generation, question answering, and insight analysis.

### Structure

- Each `##` heading becomes a separate chunk in the vector store
- Write in full sentences with rich detail — more context means smarter agents
- Include key concepts, definitions, and relationships

### Example Outline

```markdown
# Cloud Native .NET — Session Outline

## Introduction

This session covers modern patterns for building cloud-native applications
with .NET — containers, orchestration with Kubernetes, and observability
with OpenTelemetry.

## Containers and .NET

.NET 8 introduced built-in container publishing with `dotnet publish`.
This eliminates the need for a Dockerfile in many scenarios. The SDK
generates optimized, multi-layer images with proper signal handling and
health check support.

Common misconception: containers add significant overhead. In practice,
.NET containers add <10ms startup latency compared to bare-metal, and
memory overhead is negligible because containers share the host kernel.

## Orchestration with Kubernetes

Kubernetes provides declarative infrastructure for running containerized
workloads. Key concepts include Pods, Deployments, Services, and
ConfigMaps. .NET Aspire simplifies local development by modeling these
relationships in C#.
```

### Tips for a Great Outline

- **The richer your outline, the smarter the AI agents become.** Don't hold back — write everything you'd want a knowledgeable assistant to know about your talk.
- **Include common misconceptions.** Agents use these to generate more insightful polls (e.g., "Which of these is a myth about containers?").
- **Include related topics and connections.** Agents will draw on these during analysis to surface cross-cutting themes.
- **Use `##` headings intentionally.** Each `##` section becomes its own chunk — group related ideas under the same heading for coherent retrieval.

---

## Step 4: Test Your Customizations

After editing all three files, verify everything works end-to-end:

1. **Stop the running app** — press Ctrl+C or run `aspire stop`
2. **Restart** — run `aspire run`
3. **Open the presenter dashboard** — verify your topics appear with the correct titles and order
4. **Navigate slides** — verify they display correctly and map to the right topics
5. **Generate a poll** — verify the suggested polls appear and that AI-generated polls are relevant to your content
6. **Check knowledge base stats** — verify your outline was ingested (record count should be > 0)

> 💡 If topics don't appear, double-check your JSON syntax. A missing comma or bracket in `seed-topics.json` will silently fail. Use `cat data/seed-topics.json | python -m json.tool` to validate.

---

## Step 5: Create a Session from GitHub (Alternative)

Don't want to write everything by hand? Conference Pulse can draft a session from a GitHub repository:

1. Go to `/create` in the web app
2. Select **Import from GitHub**
3. Enter a GitHub repository URL (e.g., `https://github.com/dotnet/aspire`)
4. Choose your generation options:
   - ☑️ Generate slides with speaker notes
   - ☑️ Generate suggested polls
   - ☑️ Generate talking points
5. The app fetches the repository's Markdown files, ingests them into the knowledge base, and uses AI to draft topics, polls, and a slide deck
6. Review and edit the generated content before starting your session

> 💡 GitHub import gives you a solid starting point. You'll almost always want to refine the generated topics, reorder slides, and add your own speaker notes. Think of it as a first draft, not the final product.

---

## Tips for Great Content

| Area | Guidance |
|------|----------|
| **Topics** | 3–5 topics is ideal. Each should cover 10–20 minutes of your talk. |
| **Suggested polls** | Write 2–3 per topic. The AI can generate more, but your hand-crafted polls ensure quality. |
| **Slides** | Mix slide types for visual variety — Title, Section, Content, Code, and Blank for demo pauses. |
| **Knowledge base** | More context = smarter AI. Write the outline as if you're briefing a co-presenter. |
| **Testing** | Always run the full flow before your talk. Generate polls, test Q&A, verify insights generate correctly. |

---

## Example: Adapting for a "Cloud Native .NET" Talk

Let's walk through customizing all three files for a hypothetical 45-minute talk about cloud-native .NET development.

### 1. Define Your Topics (`data/seed-topics.json`)

```json
{
  "sessionId": "CLOUDNATIVE-2025",
  "title": "Cloud Native .NET",
  "description": "Containers, orchestration, and observability for modern .NET applications.",
  "topics": [
    {
      "id": "containers",
      "title": "Containers & .NET",
      "description": "Building and publishing .NET containers — with and without Dockerfiles",
      "order": 1,
      "talkingPoints": [
        "dotnet publish container support in .NET 8+",
        "Multi-stage builds vs SDK publish",
        "Health checks and graceful shutdown"
      ],
      "suggestedPolls": [
        {
          "question": "How do you build your .NET containers today?",
          "options": ["Dockerfile", "dotnet publish", "CI/CD pipeline handles it", "Not using containers yet"]
        }
      ]
    },
    {
      "id": "orchestration",
      "title": "Orchestration",
      "description": "Running .NET workloads on Kubernetes and with .NET Aspire",
      "order": 2,
      "talkingPoints": [
        "Kubernetes primitives (Pods, Deployments, Services)",
        ".NET Aspire for local dev orchestration",
        "Scaling strategies for .NET services"
      ],
      "suggestedPolls": [
        {
          "question": "What's your primary orchestration platform?",
          "options": ["Kubernetes", "Docker Compose", ".NET Aspire", "Azure Container Apps", "None yet"]
        }
      ]
    },
    {
      "id": "observability",
      "title": "Observability",
      "description": "Logs, metrics, and traces with OpenTelemetry in .NET",
      "order": 3,
      "talkingPoints": [
        "OpenTelemetry SDK for .NET",
        "Structured logging with ILogger",
        "Distributed tracing across services",
        "Metrics and dashboards"
      ],
      "suggestedPolls": [
        {
          "question": "Which observability pillar do you struggle with most?",
          "options": ["Logging", "Metrics", "Distributed tracing", "Correlating all three"]
        }
      ]
    }
  ]
}
```

### 2. Author Your Slides (`data/slides.md`)

```markdown
<!-- layout: centered -->

# Cloud Native .NET

## Containers, Orchestration & Observability

<!-- speaker: Welcome! 45 minutes. Three segments. Their phones are their remote control. -->

---

<!-- layout: centered -->

## Scan to Join

### 📱 Vote, ask questions, shape this session live

<!-- speaker: Wait 30 seconds for people to scan the QR code. -->

---

<!-- topic: containers -->
<!-- layout: centered -->

# Containers & .NET

## Build Once, Run Anywhere

<!-- speaker: 15 minutes for this segment. -->

---

## Container Publishing in .NET 8+

- `dotnet publish -t:PublishContainer` — no Dockerfile needed
- Multi-layer images optimized by the SDK
- Automatic signal handling and health checks
- Works with any container registry

<!-- speaker: Demo: publish a container live from the terminal. -->

---

<!-- topic: orchestration -->
<!-- layout: centered -->

# Orchestration

## From Local Dev to Production

<!-- speaker: Transition to orchestration. 15 minutes. -->

---

## .NET Aspire for Local Development

- Model your distributed app in C#
- Automatic service discovery and connection strings
- Built-in dashboard for logs, traces, and metrics
- One command: `aspire run`

<!-- speaker: Show the Aspire dashboard with 3-4 services running. -->

---

<!-- topic: observability -->
<!-- layout: centered -->

# Observability

## Seeing Inside Your Services

<!-- speaker: Final segment. 15 minutes. Drive home that observability is not optional. -->

---

## OpenTelemetry in .NET

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());
```

<!-- speaker: Walk through each line. Emphasize how little code is needed. -->
```

### 3. Write Your Outline (`data/session-outline.md`)

```markdown
# Cloud Native .NET — Session Outline

## Introduction

This session covers the three pillars of cloud-native .NET development:
containers, orchestration, and observability. Attendees will see live demos
of each technology and participate in real-time polls.

## Containers and .NET

.NET 8 introduced built-in container publishing. The `dotnet publish`
command can generate optimized container images without a Dockerfile.
Images are multi-layered, with separate layers for the runtime, framework
dependencies, and application code.

Common misconception: "You always need a Dockerfile." For most .NET apps,
SDK publishing is simpler and produces smaller, more secure images.

## Orchestration with Kubernetes and Aspire

Kubernetes provides declarative infrastructure for containerized
workloads. .NET Aspire complements Kubernetes by simplifying local
development — it models service dependencies in C# and provides automatic
service discovery, connection string management, and a built-in dashboard.

## Observability with OpenTelemetry

OpenTelemetry is the industry standard for logs, metrics, and distributed
traces. .NET has first-class support through the OpenTelemetry SDK. Adding
observability requires minimal code — a few lines in Program.cs enable
full request tracing across services.
```

### The Result

After saving all three files and running `aspire run`, you'll have a fully customized session with three topics, seven slides, suggested polls, and a knowledge base primed with your content. The AI agents will generate polls about containers, answer questions about Kubernetes, and produce insights grounded in your outline.

---

## Next Steps

- [Slide Authoring Guide](../slide-authoring-guide.md) — Full reference for slide conventions, layouts, and formatting
- [Presenter Guide](../presenter-guide.md) — Tips for running a live demo on stage
- [Configuration Reference](../configuration.md) — Deployment, infrastructure, and secrets setup
