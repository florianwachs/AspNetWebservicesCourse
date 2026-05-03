# Slide Authoring Guide

## Overview

The Conference Pulse app uses a single Markdown file — **`data/slides.md`** — as the presentation deck. You write slides in plain Markdown using a small set of conventions, and the app does the rest:

- **`/display`** — the big-screen view projected for the audience (visible slide content only).
- **`/presenter`** — the speaker dashboard with slide navigation, speaker notes, and session controls.
- **AI Knowledge Base** — the same slide content is ingested into the vector store at startup, so the AI agents understand the material you're presenting.

One file. Three purposes. No slide designer needed.

---

## Quick Start

Here's a minimal three-slide deck to get you started:

````markdown
---
title: My Presentation
---

<!-- layout: centered -->

# Welcome
## My Talk Title

<!-- speaker: Welcome everyone! Keep this intro to 2 minutes. -->

---

## Key Points

- First point
- Second point
- Third point

<!-- speaker: Spend 5 minutes here. Elaborate on each point. -->

---

## Code Example

```csharp
Console.WriteLine("Hello, Conference!");
```

<!-- speaker: Demo this live in the terminal. -->
````

Save this as `data/slides.md`, restart the app, and open `/display` to see your deck.

---

## Convention Reference

| Convention | Syntax | Description |
|---|---|---|
| **Slide separator** | `---` | Exactly three dashes on their own line. Every `---` starts a new slide. |
| **Speaker notes** | `<!-- speaker: ... -->` | Notes visible only on `/presenter`. Can span multiple lines. Never shown on `/display`. |
| **Topic mapping** | `<!-- topic: id -->` | Maps slides to a topic in `data/seed-topics.json`. **Sticky** — applies to all subsequent slides until the next `<!-- topic: -->` directive. |
| **Layout hint** | `<!-- layout: centered -->` | Controls slide layout. Options: `centered`, `two-column`, or omit for default (left-aligned). |
| **YAML frontmatter** | First `---` block at the top | Deck metadata (title, etc.). Skipped entirely by the parser — not rendered as a slide. |
| **Headings** | `# H1` or `## H2` | Drive slide classification (see [Slide Types](#slide-types) below). |
| **Bullets** | `- item` or `* item` | Rendered as styled bullet points with accent-colored markers. |
| **Fenced code** | `` ```language ... ``` `` | Syntax-highlighted code block. Specify the language (e.g., `csharp`, `json`). |

### Details on separators

The `---` separator must appear **on its own line** with no other content (optional trailing whitespace is fine).

> **⚠️ Note:** The parser does not have code-fence-aware splitting. A bare `---` line inside a fenced code block **will** be treated as a slide separator. In practice this is rarely an issue because `---` seldom appears alone on a line within code. If you need `---` inside a code block, add a leading space or surrounding content on the same line to prevent it from matching.

### Details on speaker notes

Everything between `<!-- speaker:` and `-->` is captured as speaker notes:

```markdown
<!-- speaker: This is a single-line note. -->

<!-- speaker:
This is a multi-line note.
You can write as much as you want here.
Line breaks are preserved.
-->
```

Multiple `<!-- speaker: -->` blocks on the same slide are concatenated (joined with newlines).

### Details on YAML frontmatter

If the very first line of `slides.md` is `---`, the parser treats the block between the first and second `---` as YAML frontmatter and skips it. This is standard Markdown frontmatter:

```markdown
---
title: My Presentation
author: Jane Doe
date: 2025-07-15
---
```

The frontmatter is **not** rendered as a slide. Your first real slide starts after the frontmatter's closing `---`.

---

## Slide Types

The parser automatically classifies each slide based on its visible content (headings, bullets, code). Here are the five types and the Markdown that produces them.

> **Note:** The `SlideType` enum also includes a `Poll` type, but it is not assigned by the Markdown parser. Poll slides are created programmatically at runtime when a poll is launched.

### 1. Title Slide

**Rule:** `# Heading` + `## Subtitle` with no other visible content.

**Markdown:**
```markdown
<!-- layout: centered -->

# The Microsoft AI Stack for .NET
## A Living Presentation

<!-- speaker: Welcome everyone! Keep the intro to ~3 minutes. -->
```

**How it renders:** Large centered title (4rem, bold white) with a smaller subtitle beneath it (2rem, 60% opacity). This is your opening slide or major section opener.

> **Tip:** Add `<!-- layout: centered -->` for title slides — it looks best when the text is centered on the big screen.

---

### 2. Section Slide

**Rule:** `# Heading` alone — no subtitle, no bullets, no code.

**Markdown:**
```markdown
# Knowledge Engineering

<!-- speaker: Transition into the knowledge segment. 15 minutes. -->
```

**How it renders:** A single large heading (3.5rem) in the accent color (#e94560). Use section slides as visual "chapter markers" between segments of your talk.

> **Note:** A lone `## Heading` (H2 with nothing else) also produces a Section slide. The parser treats any heading-only slide as a section divider.

---

### 3. Content Slide

**Rule:** A heading (`##` recommended) followed by a bullet list.

**Markdown:**
```markdown
## Key Abstractions

- **IChatClient** — one interface for any LLM provider
- **IEmbeddingGenerator** — embeddings for search & ingestion
- **ChatClientBuilder** — composable middleware pipeline
- **AIFunctionFactory** — turn any .NET method into an AI tool

<!-- speaker: Walk through each abstraction briefly. -->
```

**How it renders:** Heading in accent color (2.5rem), followed by styled bullet points (2rem) with `▸` markers. Body text (if used instead of bullets) renders at 1.75rem.

> **Tip:** Keep to **5–6 bullets max** per slide. At 2rem font size on a 1080p screen, more than that gets crowded.

---

### 4. Code Slide

**Rule:** A heading followed by a fenced code block.

**Markdown:**
````markdown
## The Middleware Pipeline

```csharp
var openaiBuilder = builder.AddAzureOpenAIClient("openai");

openaiBuilder.AddChatClient("chat")
    .UseFunctionInvocation()
    .UseOpenTelemetry()
    .UseLogging();

openaiBuilder.AddEmbeddingGenerator("embedding");
```

<!-- speaker: Open Program.cs and show lines 32-42. This is real code from THIS app. -->
````

**How it renders:** Heading in accent color, followed by a code block (1.5rem monospace) with a dark semi-transparent background, rounded corners, and horizontal scroll if needed. The language tag (`csharp`) enables syntax highlighting.

> **Tip:** Keep code blocks to **5–10 lines max**. At 1.5rem on a projector, longer snippets become hard to read from the back of the room.

---

### 5. Blank Slide

**Rule:** Only `<!-- speaker: ... -->` comments with no visible content.

**Markdown:**
```markdown
<!-- speaker:
Demo time! Open the terminal and run `dotnet run`.
Show the audience the live output. Take 3 minutes.
-->
```

**How it renders:** Nothing visible on `/display` — the screen stays on the previous state or shows idle. The speaker notes are visible on `/presenter`. Use blank slides for demo pauses, live-coding segments, or audience interaction moments.

> **Note:** If a slide block has no speaker notes and no visible content, it is skipped entirely (not rendered at all).

---

## Speaker Notes Best Practices

Speaker notes are your private teleprompter on `/presenter`. Use them liberally — they never appear on the big screen or in the AI knowledge base.

### Timing cues

```markdown
<!-- speaker: 15 minutes for this segment. -->
<!-- speaker: You should be at ~30 minutes by this slide. -->
```

### Demo instructions

```markdown
<!-- speaker:
Open src/ConferenceAssistant.Web/Program.cs and show lines 32-42.
Say: "Three lines of middleware. Function invocation so agents can call tools."
Emphasize: This is real code from THIS application.
-->
```

### Audience interaction

```markdown
<!-- speaker: Ask: "Who has used IChatClient before?" Scan for hands. -->
```

### Transition phrases

```markdown
<!-- speaker:
Transition: "Now let's give our agents knowledge."
-->
```

### Poll instructions

```markdown
<!-- speaker:
Launch the suggested poll about AI experience levels.
Say: "Let's see where everyone is. Vote now."
Wait for poll results to come in. Comment on the distribution.
-->
```

### Multi-line notes

Everything between `<!-- speaker:` and `-->` is captured, so go wild:

```markdown
<!-- speaker:
This is the climax of the talk. Build the energy.

1. Connect Copilot to the /mcp endpoint
2. Type: "Summarize this session including poll results"
3. Watch the cascade visualization on /display
4. Let it finish before speaking

If it fails, have the backup summary ready in your notes app.
-->
```

---

## Display Design Constraints

The `/display` page is designed for 1920×1080 projection in a dark room. Keep these constraints in mind while authoring:

| Element | Font Size | Max Recommended |
|---|---|---|
| Title (H1 on title slides) | 4rem (~64px) | ~8 words |
| Subtitle (H2 on title slides) | 2rem (~32px) | ~12 words |
| Section heading (standalone H1) | 3.5rem (~56px) | ~6 words |
| Content heading (H2) | 2.5rem (~40px) | ~10 words |
| Bullet points | 2rem (~32px) | 5–6 per slide |
| Body text | 1.75rem (~28px) | 3–4 short paragraphs |
| Code blocks | 1.5rem (~24px) | 5–10 lines |

### Color scheme

- **Background:** `#0a0a0a` (near-black)
- **Text:** `#ffffff` (white)
- **Accent (headings, bullet markers):** `#e94560` (coral red)
- **Code background:** `rgba(255, 255, 255, 0.05)` (subtle dark overlay)
- **Fonts:** Segoe UI / system sans-serif for text; Cascadia Code / Fira Code / JetBrains Mono for code

### Other constraints

- **No images** — the display renderer supports text and code only. No `![alt](url)` image syntax.
- **No tables** — Markdown tables are not styled for display. Use bullet points instead.
- **Bold/italic in bullets** — `**bold**` and `*italic*` work inside bullet text and body markdown.
- **Links** — rendered as text but not clickable on the big screen. Use them for reference URLs that the audience can photograph.

---

## Topic Mapping

The `<!-- topic: id -->` directive connects slides to topics defined in `data/seed-topics.json`. This powers topic-aware navigation in the presenter dashboard.

### How it works

1. Place `<!-- topic: meai -->` on the first slide of a segment.
2. **All subsequent slides** inherit that topic ID — it's "sticky."
3. To switch topics, use another `<!-- topic: agents -->` directive.
4. To clear the topic (for outro slides, etc.), you can simply leave slides without any topic directive before the first `<!-- topic: -->` appears.

### Example

```markdown
<!-- topic: meai -->
<!-- layout: centered -->

# Microsoft.Extensions.AI
## The Common Language

<!-- speaker: Start of the M.E.AI segment. 15 minutes. -->

---

## Key Abstractions

- **IChatClient** — one interface for any LLM provider
- **IEmbeddingGenerator** — embeddings for search & ingestion

<!-- speaker: Walk through each abstraction. -->

---

<!-- topic: knowledge -->
<!-- layout: centered -->

# Knowledge Engineering
## DataIngestion + VectorData

<!-- speaker: Transition to the knowledge segment. -->
```

In this example, the first two slides belong to topic `meai`. The third slide starts topic `knowledge`.

### Available topic IDs

These must match the `id` field in `data/seed-topics.json`:

| Topic ID | Title | Description |
|---|---|---|
| `meai` | Microsoft.Extensions.AI | Abstractions — IChatClient, IEmbeddingGenerator, etc. |
| `knowledge` | Knowledge Engineering | DataIngestion pipelines and VectorData stores |
| `agents` | Agentic AI | Agent Framework — ChatClientAgent, tools, workflows |
| `mcp` | Interoperability with MCP | Model Context Protocol — MCP server and client |
| `closer` | The Closer | Full-stack cascade demo and wrap-up |

### What topic mapping enables

- **Auto-navigation:** When the presenter activates a topic (clicking "Activate" in the dashboard), the display auto-navigates to the first slide for that topic.
- **Contextual polls:** The Survey Architect agent uses the active topic to generate relevant poll questions.
- **Scoped knowledge:** Topic IDs let agents understand which segment of the talk is currently active.

---

## Dual-Purpose: Slides + Knowledge Base

Your slide content does double duty. The DataIngestion pipeline reads `slides.md` at startup and feeds the visible content into the AI knowledge base (vector store).

### What gets ingested

- ✅ **Headings** — slide titles and subtitles
- ✅ **Bullet points** — all bullet text
- ✅ **Code blocks** — code snippets with language context
- ✅ **Body text** — any non-bullet paragraph content

### What does NOT get ingested

- ❌ **Speaker notes** — `<!-- speaker: ... -->` is an HTML comment. The Markdown parser (Markdig) strips HTML comments automatically, so notes never enter the vector store.
- ❌ **Directives** — `<!-- topic: ... -->` and `<!-- layout: ... -->` are also HTML comments and are stripped.
- ❌ **YAML frontmatter** — skipped by the parser.

### Why this matters

The richer your slide content, the smarter the AI agents become. When someone in the audience asks "What is IChatClient?", the agent can find the answer in the ingested slide content. Write clear, descriptive bullet points — they serve the audience on screen AND the AI behind the scenes.

---

## Editing Workflow

### Getting started

1. **Edit** `data/slides.md` in VS Code (the built-in Markdown preview works great for quick checks).
2. **Restart** the app to reload the slide deck (`dotnet run` or restart via Aspire).
3. **Test navigation** at `/presenter` — click through slides and verify speaker notes.
4. **Check rendering** at `/display` — open in a separate browser window (ideally at 1920×1080) to see how slides look on the big screen.

### Tips

- Use **VS Code split view** — Markdown source on the left, preview on the right.
- The Markdown preview won't match the dark-theme display exactly, but it's good for catching syntax errors and previewing structure.
- Keep a browser tab open to `/display` while editing. After each restart, you'll see your changes immediately.
- **Hot-reload is not yet supported** — you must restart the app to pick up changes to `slides.md`. (Planned for Phase 2.)

### Common mistakes

| Mistake | Symptom | Fix |
|---|---|---|
| Missing `---` separator | Two slides merge into one | Add `---` on its own line between slides |
| `---` inside a code block | Slide splits unexpectedly | The parser does not skip `---` inside code fences. Add a leading space or surrounding content so the line isn't a bare `---`. |
| Forgetting the closing `-->` | Speaker notes leak into visible content | Ensure every `<!-- speaker:` has a matching `-->` |
| Too many bullets | Text overflows or shrinks | Keep to 5–6 bullets max per slide |
| Code block too long | Audience can't read from the back | Keep to 5–10 lines; trim to the essential parts |
| Wrong topic ID | Topic activation doesn't navigate to the slide | Check that the ID matches `data/seed-topics.json` exactly |

---

## Full Example

Here's a complete topic segment (5 slides) using all the conventions, taken from the Knowledge Engineering section of the actual deck:

````markdown
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
````

This produces:
1. A **Title** slide (centered, H1 + H2) — the segment opener.
2. A **Content** slide (H2 + numbered bullets) — the pipeline overview.
3. A **Code** slide (H2 + fenced C# block) — pipeline code.
4. A **Code** slide (H2 + fenced C# block) — search code.
5. A **Content** slide (H2 + bullet list) — the knowledge base concept.

All five slides are mapped to topic `knowledge`, so activating that topic in the presenter dashboard navigates straight to slide #1 of this segment.

---

## Related Documentation

- [Getting Started](getting-started.md) — setup, configuration, and first run
- [Presenter Guide](presenter-guide.md) — how to use the presenter dashboard during a live talk
- [Tutorial: Customize for Your Talk](tutorials/03-customize-for-your-talk.md) — adapt topics, slides, and content for your presentation
- [Tutorial: Running a Live Demo](tutorials/02-running-a-live-demo.md) — step-by-step walkthrough of a live session
- [Configuration](configuration.md) — environment variables and app settings
- [Architecture](architecture.md) — how slides, agents, and the knowledge base fit together
