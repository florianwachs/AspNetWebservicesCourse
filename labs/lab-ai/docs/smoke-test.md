# Conference Pulse — Smoke Test Checklist

> **Looking for setup instructions?** See [Getting Started](getting-started.md).
> **Want a guided walkthrough?** See [Your First Session](tutorials/01-your-first-session.md).
> **Having issues?** See [Troubleshooting](troubleshooting.md).

This checklist verifies that all Conference Pulse features are working correctly. Run through it after setup or code changes.

---

## 1. Aspire Dashboard — `http://localhost:18888`

| # | Check | Expected |
|---|-------|----------|
| 1 | Dashboard loads | Login page → click through with token from console |
| 2 | Resources tab | Shows **`web`** (ASP.NET Core), **`openai`** (Azure OpenAI), **`postgres`** (PostgreSQL + pgvector), **`conferencedb`** (database), and **`postgres-pgweb`** (pgWeb admin) |
| 3 | `web` resource status | Running / Healthy |
| 4 | `openai` resource status | Healthy (if secrets configured) |
| 5 | `postgres` resource status | Running (Docker container) |
| 6 | `postgres-pgweb` endpoint | Click to open pgWeb — browse `conferencedb` tables (sessions, polls, etc.) |
| 7 | Click `web` endpoint link | Opens the app in browser |
| 8 | Structured Logs tab | Shows startup messages including database schema creation |
| 9 | Traces tab | Shows HTTP request traces with OpenTelemetry spans |

---

## 2. Home Page — `/`

| # | Check | Expected |
|---|-------|----------|
| 1 | Page loads | Conference Pulse landing page appears |
| 2 | Active sessions list | Default session visible (auto-created at startup) |
| 3 | Session cards | Each shows title, session code, status, and attendee count |
| 4 | "Create New Session" button | Visible and navigates to `/create` |
| 5 | Click a session card | Expands/shows join options (Presenter, Attendee, Display) |
| 6 | Click "Presenter Dashboard" | Navigates to `/presenter/{SessionCode}` |
| 7 | Click "Join Session" | Navigates to `/session/{SessionCode}` |
| 8 | Click "Projection Display" | Navigates to `/display/{SessionCode}` |

---

## 3. Presenter Dashboard — `/presenter/{SessionCode}`

Enter the host PIN when prompted (default session: `0000`).

### 3a. Setup State

| # | Check | Expected |
|---|-------|----------|
| 1 | Session title | "The Microsoft AI Stack for .NET" |
| 2 | Status badge | "Setup" |
| 3 | KB counter | "📚 X records" (X > 0 if AI configured) |
| 4 | Topics in left column | 5 topics: meai, knowledge, agents, mcp, closer |
| 5 | "🚀 Go Live" button | Visible |
| 6 | Topic activate buttons | NOT visible (session not live) |
| 7 | 3-column layout | Left: topics, Center: slides, Right: Q&A + polls + insights |

### 3b. Go Live

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **🚀 Go Live** | Status → "Live" |
| 2 | Button changes | Shows "⏹ End Session" |
| 3 | Topics | Each shows **▶ Activate** button |

### 3c. Topic Lifecycle

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **▶ Activate** on first topic | Topic active, left column highlights it |
| 2 | Active topic badge | "Active" status |
| 3 | Other topics | "Upcoming" |
| 4 | Click **✓ Complete** | Status → "Completed" |
| 5 | Activate next topic | Previous stays completed |
| 6 | Navigate slides past topic boundary | Topic auto-activates (SyncTopicToSlide) |

### 3d. Polls — Suggested

| # | Action | Expected |
|---|--------|----------|
| 1 | Activate topic "meai" | Poll section appears |
| 2 | Dropdown shows suggested polls | Pre-configured polls listed |
| 3 | Select + click **Launch** | Poll appears (all counts 0) |
| 4 | Poll status | "Draft" with **📡 Go Live** button |
| 5 | Click **📡 Go Live** | Poll becomes Active |
| 6 | Click **🔒 Close Poll** | Poll closes, results frozen |

### 3e. Polls — Auto-Generate (🤖 requires AI)

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **🤖 Auto-Generate Poll** | AI generates a contextual poll |
| 2 | Poll appears | Question + options rendered |

### 3f. Polls — Custom

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **✏️ Custom Poll** | Form with question + 2 option fields |
| 2 | Fill in question + ≥2 options | **🚀 Create & Go Live** enabled |
| 3 | Click **🚀 Create & Go Live** | Custom poll created and live |

---

## 4. Attendee Session — `/session/{SessionCode}`

### 4a. Before Go Live

| # | Check | Expected |
|---|-------|----------|
| 1 | Navigate to page | "⏳ Session hasn't started yet. Hang tight!" |

### 4b. Voting (after Go Live + active poll)

| # | Action | Expected |
|---|--------|----------|
| 1 | Click a poll option | Vote registered |
| 2 | Results update | Vote count increments |
| 3 | Check Presenter | Updated counts + percentages in right column |

### 4c. Questions + AI Auto-Answer

| # | Action | Expected |
|---|--------|----------|
| 1 | Type in "Ask a Question" box + click **Send** | Question submitted, input clears |
| 2 | Question in "🔥 Top Questions" | With 👍 0 count |
| 3 | Click 👍 on a question | Count increments |
| 4 | Check Presenter | Question in "❓ Audience Questions" |
| 5 | Wait 5-10 seconds (🤖 requires AI) | AI answer appears (blue-tinted, AI badge) |

### 4d. Answering / Overriding (from Presenter)

| # | Action | Expected |
|---|--------|----------|
| 1 | No AI answer yet → click **💬 Answer** | Type answer + submit |
| 2 | AI answer exists → click **✏️ Override** | Text input replaces AI answer |
| 3 | Submit human answer | 💬 badge shown (no AI badge) |

---

## 5. Projection Display — `/display/{SessionCode}`

| # | Check | Expected |
|---|-------|----------|
| 1 | Before Go Live | Large QR code with "Scan to Join" + session code |
| 2 | After Go Live | Session title + active topic header; sidebar QR code |
| 3 | When poll active | Live bar chart renders |
| 4 | Vote from attendee session | Display updates with new counts |
| 5 | When slide active | Slide full-screen; smaller QR in sidebar |
| 6 | When idle (no poll/slide) | Shows QR code |

---

## 6. Slide System

### 6a. Presenter Slide Navigation

| # | Action | Expected |
|---|--------|----------|
| 1 | Activate first topic | Center column shows slide preview |
| 2 | Speaker notes | Visible below preview (🎤 icon) |
| 3 | Click **Next ▶** / **◀ Previous** | Slide advances/goes back |
| 4 | Progress indicator | "Slide X of Y" |
| 5 | "Up Next" preview | Shows next slide |
| 6 | Left column | Auto-highlights current topic |

### 6b. Display Slide Rendering

| # | Check | Expected |
|---|-------|----------|
| 1 | Slide active on display | Full-screen with dark background, QR sidebar |
| 2 | Large text | Readable from back of room |
| 3 | Progress dots | Current position visible |
| 4 | Advance on presenter | Display updates in real-time |

### 6c. Keyboard Navigation

| Key | Action |
|-----|--------|
| **→** or **Space** | Next slide |
| **←** | Previous slide |
| **P** | Quick-launch poll |
| **Esc** | Close active overlay |

> Keyboard navigation does NOT trigger when typing in text inputs.

### 6d. Poll/Slide Priority

| # | Action | Expected |
|---|--------|----------|
| 1 | Launch poll while slide showing | Poll **replaces** slide on display |
| 2 | Close poll | Slide **returns** on display |

### 6e. Topic Auto-Navigation

| # | Action | Expected |
|---|--------|----------|
| 1 | Activate a different topic | Slide jumps to that topic's first slide |
| 2 | Navigate past topic boundary | Topic auto-activates (SyncTopicToSlide) |

### 6f. Speaker Notes Privacy

| # | Check | Expected |
|---|-------|----------|
| 1 | `/presenter/{code}` | Speaker notes visible |
| 2 | `/display/{code}` | No notes — slide content only |

---

## 7. Multi-Session

### 7a. Create a New Session

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **"Create New Session"** on `/` | Navigates to `/create` |
| 2 | Fill in title + PIN (4-6 digits) | Required fields |
| 3 | Click **Create** | New session appears on home page |

### 7b. PIN Gate

| # | Action | Expected |
|---|--------|----------|
| 1 | Navigate to `/presenter/{code}` | PIN input appears |
| 2 | Enter incorrect PIN | Error, dashboard stays locked |
| 3 | Enter correct PIN | Dashboard unlocks |
| 4 | Refresh tab | No re-prompt (same Blazor circuit) |
| 5 | Open new tab to same URL | PIN gate appears again |

### 7c. Session Isolation

| # | Action | Expected |
|---|--------|----------|
| 1 | Create two sessions | Both on home page |
| 2 | Launch poll in session A | Only session A sees it |
| 3 | Submit question in session B | Only session B presenter sees it |

---

## 8. Real-Time Ingestion (🤖 requires AI)

| # | Action | Expected |
|---|--------|----------|
| 1 | Check initial KB count on Presenter | ~20 records (outline chunks) |
| 2 | Launch poll → vote → close | KB count increases (poll results ingested) |
| 3 | Submit question → wait for AI answer | KB count increases (Q&A pair ingested) |
| 4 | Complete a topic | KB count increases (insights ingested) |
| 5 | MCP `search_session_knowledge` | Returns results from polls, Q&A, insights — not just outline |

---

## 9. Insight Generation (🤖 requires AI)

| # | Action | Expected |
|---|--------|----------|
| 1 | Close a poll | 📊 Poll Analysis insight appears (5-10s) |
| 2 | Complete a topic | 📋 Topic Summary + 🔍 Knowledge Gap insights (5-10s) |
| 3 | Presenter "💡 Insights" section | Insight cards with type badges |
| 4 | KB count | Increases after each insight |

---

## 10. MCP Server — `/mcp`

Get the base URL from the Aspire dashboard (`web` resource endpoint).

### 10a. Tool Discovery

| # | Action | Expected |
|---|--------|----------|
| 1 | POST `initialize` to `/mcp` | Server info returned |
| 2 | POST `tools/list` to `/mcp` | 10 tools listed |

**Expected tools:**

| Tool | Params | Requires AI |
|------|--------|-------------|
| `get_session_status` | none | No |
| `get_active_poll` | none | No |
| `get_poll_results` | `pollId` | No |
| `search_session_knowledge` | `query`, `maxResults?` | Yes |
| `get_audience_questions` | `count?` | No |
| `get_topic_insights` | `topicId` | No |
| `get_all_insights` | none | No |
| `generate_session_summary` | none | Yes |
| `search_knowledge` | `query` | Yes |
| `get_knowledge_stats` | none | No |

### 10b. Tool Calls

| # | Action | Expected |
|---|--------|----------|
| 1 | Call `get_session_status` | Returns session title, status, 5 topics |
| 2 | Call `search_session_knowledge` (🤖 AI) | Returns matching KB records |
| 3 | Call `generate_session_summary` (🤖 AI) | Returns comprehensive session summary |

### 10c. VS Code / Copilot Integration

| # | Check | Expected |
|---|-------|----------|
| 1 | `.vscode/mcp.json` exists | Pre-configured with ConferencePulse server |
| 2 | Port matches Aspire dashboard | Update if needed |

---

## 11. Data Persistence — PostgreSQL

### 11a. Database via pgWeb

| # | Action | Expected |
|---|--------|----------|
| 1 | Open `postgres-pgweb` endpoint from dashboard | pgWeb UI loads |
| 2 | Select `conferencedb` | Tables: sessions, session_topics, slides, polls, poll_responses, audience_questions, question_answers, insights |
| 3 | `SELECT * FROM sessions` | Default session visible |
| 4 | `SELECT * FROM session_topics` | 5 topics with JSONB data |

### 11b. Restart Persistence

| # | Action | Expected |
|---|--------|----------|
| 1 | Create data (polls, votes, questions) | Visible in Presenter |
| 2 | Stop app → restart | Data intact, KB count preserved |

### 11c. Clear Runtime Data

| # | Action | Expected |
|---|--------|----------|
| 1 | `ClearRuntimeDataAsync` runs | Polls, responses, questions, answers, insights cleared |
| 2 | Session/topic/slide structure | Preserved |
| 3 | Knowledge base | Unchanged |

---

## 12. GitHub Repository Import

### 12a. Create Session from GitHub

| # | Action | Expected |
|---|--------|----------|
| 1 | Click **"🎯 Create a Session"** → select **"🐙 Import from GitHub"** | Import form appears |
| 2 | Enter repo URL → click **"🔍 Fetch & Draft Session"** | Import + AI drafting (10-30s) |
| 3 | Verify import stats | Document count shown |
| 4 | Verify AI-drafted topics | Topics with talking points + suggested polls |
| 5 | Click **"🚀 Create Session"** | Redirected to presenter with imported topics |
| 6 | Slides generated | Title, Section, Content, Poll, and Thank You slides |

### 12b. Import During Session

| # | Action | Expected |
|---|--------|----------|
| 1 | Expand **"📥 Import"** in left column | Import section visible |
| 2 | Paste URL → click **"📥 Import Repository"** | Import completes, history entry appears |
| 3 | KB record count | Increases |
| 4 | Click **"➕ Add All Topics"** | New topics appear in Topics panel |

---

## 13. Feature Matrix

| Feature | Without AI | With AI |
|---------|:----------:|:-------:|
| Session/topic management | ✅ | ✅ |
| Manual/suggested polls | ✅ | ✅ |
| Voting + results display | ✅ | ✅ |
| Submit/upvote questions | ✅ | ✅ |
| Manual answers | ✅ | ✅ |
| MCP tool discovery + basic tools | ✅ | ✅ |
| PostgreSQL persistence | ✅ | ✅ |
| AI auto-answer questions | ❌ | ✅ |
| Auto-generate polls | ❌ | ✅ |
| Insight generation | ❌ | ✅ |
| Real-time KB ingestion | ❌ | ✅ |
| Session summary | ❌ | ✅ |
| Semantic search | ⚠️ empty | ✅ |

---

## Quick Checklist

```
BUILD & LAUNCH
[ ] dotnet build — 0 errors
[ ] aspire run — dashboard at http://localhost:18888
[ ] Resources: web (Healthy), openai (Healthy), postgres (Running)
[ ] Console: "Default session created: XXXXXXXX (PIN: 0000)"
[ ] Console: "Ingested N outline chunks into knowledge base"

PAGES
[ ] Home (/) — lists sessions + "Create New Session"
[ ] Presenter (/presenter/{code}) — PIN gate → 3-column dashboard
[ ] Display (/display/{code}) — QR code before Go Live
[ ] Session (/session/{code}) — attendee view

CORE FLOW
[ ] Go Live → status "Live"
[ ] Activate topic → "Active"
[ ] Launch + activate poll → visible to attendees
[ ] Vote → real-time count update on Presenter + Display
[ ] Submit question → appears on Presenter
[ ] Close poll → results frozen

SLIDES
[ ] Slide navigation (Next/Prev + keyboard)
[ ] Display syncs with Presenter
[ ] Topic auto-navigation (SyncTopicToSlide)
[ ] Speaker notes only on Presenter
[ ] Poll replaces slide on Display; returns after close

AI FEATURES (requires Azure OpenAI)
[ ] AI auto-answer (🤖 badge, 5-10s)
[ ] Override AI answer (✏️ Override)
[ ] Poll Analysis insight on close
[ ] Topic Summary + Knowledge Gap on complete
[ ] KB count grows as events fire
[ ] Auto-generate poll
[ ] MCP generate_session_summary

MCP SERVER
[ ] POST /mcp initialize → server info
[ ] POST /mcp tools/list → 10 tools
[ ] POST /mcp tools/call get_session_status → data
[ ] .vscode/mcp.json configured

MULTI-SESSION
[ ] Create session with custom code + PIN
[ ] PIN gate works (reject wrong, accept correct)
[ ] Sessions isolated

DATA PERSISTENCE
[ ] Data survives app restart
[ ] KB records persist (pgvector)

GITHUB IMPORT
[ ] Import from GitHub on /create
[ ] Import during session (📥 Import section)
[ ] Slides generated from import
[ ] KB count increases after import
```
