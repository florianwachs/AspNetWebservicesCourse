# Tutorial: Your First Session

> **Prerequisites:** This tutorial assumes you've completed the [Getting Started](../getting-started.md) guide and have Conference Pulse running via `aspire run`.

## What You'll Learn

In this tutorial, you'll run through a complete Conference Pulse session lifecycle — from creating a session to generating a comprehensive AI summary. You'll experience the app from all three perspectives: **presenter**, **attendee**, and **display**.

By the end, you'll have seen every major technology in action:

| Technology | What You'll See |
|---|---|
| **Microsoft.Extensions.AI** | Every LLM call routed through `IChatClient` |
| **DataIngestion** | Content chunked, enriched, and stored automatically |
| **VectorData** | Qdrant storing and searching vector embeddings |
| **Agent Framework** | Survey Architect, Response Analyst, and Knowledge Curator collaborating |
| **MCP** | Session data exposed for external tool access |

---

## Step 1: Open the Three Views

Open three browser tabs using the URL from your Aspire dashboard:

1. 🏠 **Home** (`/`) — Your starting point
2. 🎙️ **Presenter Dashboard** — Click "Present" on the default session (DOTNETAI-CONF) or go to `/presenter/DOTNETAI-CONF`
3. 📺 **Display** — Click "Display" or go to `/display/DOTNETAI-CONF`

For the presenter dashboard, enter the PIN: **`0000`** (default for the demo session).

> 💡 **Tip:** In a real presentation, you'd have the presenter view on your laptop and the display on the projection screen. For this tutorial, arrange them as separate browser windows side by side.

---

## Step 2: Explore the Presenter Dashboard

The presenter dashboard uses a three-column layout designed for at-a-glance control:

- **Left column:** Topic list with status badges and activate/complete controls. Each topic shows its current state — Pending, Active, or Completed.
- **Center column:** Active content area — the current poll, live results, and generated insights all appear here.
- **Right column:** Session controls, a QR code for attendees to join, and knowledge base statistics showing how much context the AI has accumulated.

> 💡 **Tip:** Take a moment to look at the right column — the knowledge base stats will grow as you interact with the session. This is the AI's memory building in real time.

---

## Step 3: Go Live

Click **"Go Live"** on the presenter dashboard.

- The session status changes from **Created** to **Live**
- The display view updates to reflect the live state
- Attendees can now join and interact

> ℹ️ Until you go live, the attendee view won't show interactive elements. Going live is the starting gun for audience participation.

---

## Step 4: Activate a Topic

Click **"Activate"** on the first topic (Microsoft.Extensions.AI). Notice what happens across all three views:

- ✅ The topic badge changes to **Active** in the presenter's left column
- 📺 The display updates to show the current topic
- 🎞️ If slides are configured, the display navigates to the topic's first slide

> 💡 **What's happening:** The `SessionContext` fires a state-change event. Because Conference Pulse uses Blazor Server, all connected views receive the update instantly through the built-in circuit connection — no polling, no manual refresh.

---

## Step 5: Generate a Poll

Click **"Generate Poll"** on the active topic. Behind the scenes, a chain of AI operations fires:

1. The **Survey Architect** agent receives the topic context
2. It searches the knowledge base for relevant content using `SearchKnowledge`
3. It generates a poll question tailored to the topic and audience
4. The poll appears in **Draft** status on the presenter dashboard

> 💡 **What's happening:** This is Microsoft.Extensions.AI + Agent Framework in action. The Survey Architect agent uses `IChatClient` with function invocation to call `SearchKnowledge` and `CreatePoll` tools. The agent's system prompt (defined in `AgentDefinitions.cs`) instructs it to craft strategic, insight-gathering questions — not trivia.

---

## Step 6: Launch the Poll

Review the generated poll question and options, then click **"Launch"**. The poll:

- 📱 Appears on the attendee view (`/session/DOTNETAI-CONF`)
- 📺 Appears on the display view as a live visualization
- 📊 Vote counts update in real time as responses come in

> ℹ️ You can edit the poll question or options before launching if the AI's suggestion needs tweaking. The AI is your co-pilot, not your autopilot.

---

## Step 7: Vote as an Attendee

Switch to the attendee view (`/session/DOTNETAI-CONF`) and cast your vote on the poll. Watch what happens:

- ✅ Your vote is immediately reflected on the presenter dashboard
- 📊 The display bar chart animates to show the updated results
- 🧠 The knowledge base grows — your response is ingested as context

> 💡 **What's happening:** Your response is stored in PostgreSQL **and** ingested into the Qdrant vector store via the DataIngestion pipeline. The content is chunked, embedded, and indexed. Future polls and AI answers will have your response as additional context — this is the "snowball effect" in action.

---

## Step 8: Close the Poll and See Insights

Close the poll from the presenter dashboard. A new AI workflow kicks off:

1. The **Response Analyst** agent analyzes the poll results
2. It generates an insight — not just percentages, but **patterns and observations**
3. The insight appears on both the presenter dashboard and the display
4. The insight itself is ingested into the knowledge base for future context

> 💡 **What's happening:** The Response Analyst doesn't just summarize numbers. It searches the knowledge base for prior context, identifies trends across polls, and produces data-driven, actionable observations. Each insight makes the next one smarter.

---

## Step 9: Submit an Audience Question

Switch to the attendee view and type a question in the Q&A section. Try something like:

> _"How does IChatClient differ from using the OpenAI SDK directly?"_

Submit it, then watch:

- 📋 The question appears on the presenter dashboard
- 🤖 Within a few seconds, an AI-generated answer appears (marked with 🤖)
- 🧠 The Q&A pair is ingested into the knowledge base

> 💡 **What's happening:** The `QuestionAnsweringService` uses `IChatClient` with RAG (Retrieval-Augmented Generation). It first runs a safety check to filter inappropriate content, then searches the Qdrant vector store for relevant context, and finally generates a grounded answer. This is M.E.AI + VectorData + DataIngestion working together in a single request.

> ⚠️ **Note:** AI answers are clearly marked so the presenter and audience can distinguish them from human responses. The presenter can always provide a follow-up or correction.

---

## Step 10: Navigate Slides

Use keyboard shortcuts on the presenter dashboard to control slides:

| Key | Action |
|---|---|
| `→` or `Space` | Next slide |
| `←` | Previous slide |
| `P` | Toggle poll controls |
| `Esc` | Close / exit focus |

The display updates in sync with your navigation — what you see is what the audience sees.

> 💡 **Tip:** These shortcuts are shown in the keyboard hints bar at the bottom of the presenter view. You can control the entire presentation without touching the mouse.

---

## Step 11: Advance Topics

Complete the current topic (click **"Complete"**) and activate the next one.

Notice how the generated polls become **more contextual** — the Survey Architect now has more knowledge from previous polls, responses, and insights. This is the "snowball effect":

- **Topic 1 poll:** Based only on the topic description and seed content
- **Topic 2 poll:** Informed by Topic 1's poll results, audience responses, and generated insights
- **Topic 3+ polls:** Drawing on an increasingly rich knowledge base

> 💡 **What's happening:** The Knowledge Curator agent works behind the scenes to synthesize information across topics. Each interaction enriches the vector store, and the Survey Architect's `SearchKnowledge` tool returns increasingly relevant context.

---

## Step 12: Generate a Session Summary

When you've covered all the topics you want, click **"Generate Summary"** on the presenter dashboard. The **Knowledge Curator** agent produces a comprehensive summary that includes:

- 📋 All topics covered and their key takeaways
- 📊 Poll results with analysis and patterns
- ❓ Audience questions and answers
- 🧠 Knowledge base statistics
- 🔍 AI-generated synthesis connecting themes across the session

> 💡 **What's happening:** The Knowledge Curator searches the entire knowledge base, synthesizes content from multiple sources (polls, Q&A, insights), and produces a structured summary. This is the Agent Framework orchestrating a complex, multi-step knowledge synthesis task.

---

## What Just Happened?

Congratulations — you just ran a complete Conference Pulse session! Here's a recap of every technology that fired during your walkthrough:

| # | Technology | How It Was Used |
|---|---|---|
| 1 | **Microsoft.Extensions.AI** | Every LLM call — poll generation, insight analysis, Q&A answers, summaries — went through `IChatClient` |
| 2 | **DataIngestion** | Poll responses, Q&A pairs, and insights were automatically chunked, enriched, and stored |
| 3 | **VectorData** | Qdrant stored vector embeddings and powered semantic search for RAG |
| 4 | **Agent Framework** | Survey Architect, Response Analyst, and Knowledge Curator collaborated through tool-calling |
| 5 | **MCP** | The MCP server exposed all session data, making it accessible to external tools |

The key insight: these technologies aren't isolated features — they form a **feedback loop**. Each audience interaction enriches the knowledge base, which makes the next AI operation smarter. That's the demo story Conference Pulse tells.

---

## Next Steps

Now that you've experienced a full session lifecycle, explore further:

- 📖 **[Tutorial: Running a Live Demo](02-running-a-live-demo.md)** — Practice the full demo flow with timing and talking points
- 🎨 **[Tutorial: Customize for Your Talk](03-customize-for-your-talk.md)** — Configure topics, slides, and seed content for your own presentation
- 🔬 **[Technology Guide](../technology-guide.md)** — Deep dive into each technology and how it's implemented
- 🔌 **[MCP Reference](../mcp-reference.md)** — Connect external tools to Conference Pulse via the MCP server
