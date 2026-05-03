# Conference Pulse — Product Requirements Document

## 1. Product Overview

**Conference Pulse** is a live, interactive presentation platform that evolves the "living presentation" concept beyond traditional slides. The app **is** the presentation — but it now includes Markdown-authored visual content that's richer than anything PowerPoint offers: dynamic, navigable slides with speaker notes, code layouts, and topic-aware content — all rendered full-screen on the big screen while the speaker controls pacing from the presenter view. This isn't going back to static decks — it's structured visual content layered on top of an AI-powered web experience. It demonstrates six Microsoft AI technologies working together by being the actual demo — the audience participates in the app while the speaker explains how it works.

The app generates polls, analyzes responses, fetches external documentation, ingests knowledge in real-time, and synthesizes insights — all powered by agents, all visible to the audience, all running live.

---

## 2. User Personas

### Speaker (1 person)
- Controls the presentation flow from `/presenter`
- Triggers poll generation, advances topics, sees agent activity
- Has full visibility into what agents are doing behind the scenes
- Explains the technology while it runs

### Attendee (10–500 people)
- Joins via QR code at `/session/{code}`
- Votes on polls, asks questions, sees results
- Mobile-first experience (phone browser)
- No login required — anonymous participation with optional nickname

### Display (1 projection screen)
- Shows at `/display` on the big screen
- Auto-updating: polls animate, insights appear, results stream in
- Designed for readability at distance (large fonts, high contrast)
- No interaction — purely observational

---

## 3. User Stories

### Speaker Stories

| ID | Story | Acceptance Criteria |
|----|-------|-------------------|
| S1 | As a speaker, I can load a session from an outline so that topics are pre-populated | Session outline ingested at startup; topics visible in presenter dashboard |
| S2 | As a speaker, I can trigger poll generation for the current topic | Clicking "Generate Poll" triggers SurveyArchitect agent; poll appears within 10s |
| S3 | As a speaker, I can launch a poll to the audience | Active poll appears on attendee devices and display |
| S4 | As a speaker, I can close a poll and trigger analysis | Closing triggers ResponseAnalyst agent; insight appears within 15s |
| S5 | As a speaker, I can advance to the next topic | Topic changes; display updates; knowledge context shifts |
| S6 | As a speaker, I can see agent activity in real-time | Activity log shows what each agent is doing, which tools it's calling |
| S7 | As a speaker, I can trigger a session summary | Triggers SessionSummaryWorkflow; summary streams to display |
| S8 | As a speaker, I can author presentation content in Markdown so that I write naturally without editing JSON | Slides authored in `data/slides.md` using `---` separators; parsed at startup |
| S9 | As a speaker, I can navigate slides from the presenter view with keyboard shortcuts so that I control pacing | Next/prev/jump-to-slide via keyboard; display updates in real-time |
| S10 | As a speaker, I see speaker notes that the audience doesn't, including timing cues and demo instructions | `<!-- speaker: -->` comments visible only in presenter view; hidden from display and attendees |
| S11 | As a speaker, I can see what the display is currently showing (slide preview in presenter view) | Presenter view includes a live preview of the current display slide |
| S12 | As a speaker, I can see the next upcoming slide for smooth transitions | Presenter view shows next slide preview alongside current slide |

### Attendee Stories

| ID | Story | Acceptance Criteria |
|----|-------|-------------------|
| A1 | As an attendee, I can join a session by scanning a QR code | QR leads to `/session/{code}`; join is instant, no login |
| A2 | As an attendee, I can vote on active polls | Poll UI shows on phone; tap to vote; confirmation shown |
| A3 | As an attendee, I can see live poll results | Results animate in real-time as votes come in |
| A4 | As an attendee, I can ask questions | Text input; question appears in feed; may get AI-generated answer |
| A5 | As an attendee, I can upvote other questions | Questions have upvote count; sorted by popularity |

### Display Stories

| ID | Story | Acceptance Criteria |
|----|-------|-------------------|
| D1 | As a display, I show the current topic prominently | Topic title + description visible from back of room |
| D2 | As a display, I show live poll voting in real-time | Bar chart animates as votes arrive |
| D3 | As a display, I show AI-generated insights | Insight panel updates when agents produce analysis |
| D4 | As a display, I show the question feed | Top questions visible, sorted by upvotes |
| D5 | As a display, I show the streaming session summary | Summary text streams in word-by-word during the closer |
| D6 | As a display, I show the current slide content full-screen with large, readable text | Slide content renders at display-optimized font sizes; readable from back of room |
| D7 | As a display, I show slides with different layouts based on content type (title, bullets, code) | Layout auto-detected from slide content; title slides centered, code slides use monospace |
| D8 | As a display, I show a subtle progress indicator for slide position | Progress bar or "3 / 12" indicator visible but unobtrusive |

---

## 4. Functional Requirements

### FR1: Session Management
- Load session from `session-outline.md` at startup
- Pre-populate topics from `seed-topics.json`
- Session state is entirely in-memory (single session lifetime)
- Session code (e.g., "AICONF") for audience to join

### FR2: Poll System
- Polls have: question, 2-6 options, status (Draft/Active/Closed)
- One active poll at a time
- Votes are anonymous (optional attendee nickname for analytics)
- Real-time vote count updates via SignalR

### FR3: AI Poll Generation
- Given a topic, `SurveyArchitect` agent generates a poll
- Agent searches vector store for context before generating
- Generated poll goes to Draft status; speaker reviews and launches

### FR4: AI Response Analysis
- When poll closes, `ResponseAnalyst` analyzes results
- Produces an `Insight` with patterns, misconceptions, audience sentiment
- `KnowledgeCurator` may add supporting documentation links

### FR5: Knowledge Ingestion
- Session outline ingested at startup (MarkdigReader → HeaderChunker)
- Audience responses ingested as they arrive (sentiment enriched)
- Audience questions ingested (keyword enriched)
- MCP-fetched docs ingested when Knowledge Curator retrieves them

### FR6: Semantic Search
- All agents can search the vector store for relevant context
- Search returns top-K results with relevance scores
- Used for: poll generation context, Q&A answers, summary generation

### FR7: MCP Server
- Exposes 6 tools via Streamable HTTP at `/mcp`
- External tools (GitHub Copilot, Claude Desktop, etc.) can connect
- `generate_session_summary` triggers the full agent workflow

### FR8: MCP Clients
- Knowledge Curator can call Microsoft Learn MCP for documentation
- Knowledge Curator can call DeepWiki MCP for GitHub repo knowledge
- Fetched content is ingested into vector store for future queries

### FR9: Copilot SDK Demo
- Separate console app
- Connects to our MCP server via `CopilotClient` + `McpServers`
- Asks for session summary; streams result to console

### FR10: Question System
- Attendees submit free-text questions
- Questions have upvote count
- Knowledge Curator can answer questions using RAG + MCP
- Answers appear in the question feed

### FR11: Slide System
- Slide deck authored in Markdown (`data/slides.md`)
- Parsed at startup by `SlideMarkdownParser`
- Navigated by presenter (next/prev/jump-to-slide)
- Rendered full-screen on display
- Slides map to topics via `<!-- topic: id -->` directives
- Speaker notes (`<!-- speaker: -->`) visible only in presenter view
- Active poll takes display priority over slides

---

## 5. Non-Functional Requirements

| Requirement | Target |
|-------------|--------|
| Concurrent attendees | 500 |
| Poll response latency | < 200ms (vote submission) |
| Agent response time | < 15s for poll generation, < 20s for analysis |
| Display update latency | < 1s via SignalR |
| Session duration | 60-90 min (all in-memory, no persistence needed) |
| Browser support | Modern mobile browsers (Chrome, Safari, Edge) |
| Accessibility | WCAG 2.1 AA for attendee and display views |
| Slide transition latency | < 200ms via SignalR |

---

## 6. Out of Scope (v1)

- User authentication / login
- Persistent storage / database
- Microsoft Teams integration
- Multi-session support (only one session at a time)
- Admin panel for managing multiple events
- Offline support
- Fluid Framework / real-time collaborative editing
