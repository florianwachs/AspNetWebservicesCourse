# Running a Live Demo

> **Prerequisites:** This tutorial assumes you've completed [Your First Session](01-your-first-session.md) and are comfortable with the basic workflow.

## What You'll Learn

How to run Conference Pulse as a live demo, including connecting external AI tools via MCP to create the **cascade effect** — where all five technologies fire in sequence. By the end of this guide, you'll have rehearsed every segment and know exactly what to do when something goes sideways on stage.

---

## The Demo Narrative: The Snowball Effect

The key concept: **each segment enriches the knowledge base, making later segments smarter.** Early segments produce raw data; later segments synthesize it. By the closer, the system has a rich, interconnected understanding of the entire session.

```
Segment 1 (M.E.AI):     outline only           → basic poll generation
Segment 2 (Knowledge):  + poll responses        → contextual understanding
Segment 3 (Agents):     + insights              → trend-aware analysis
Segment 4 (MCP):        + external docs         → maximum context
Closer:                  FULL knowledge base     → comprehensive summary
```

Think of it as a snowball rolling downhill — each segment adds mass. When you reach the closer, the system draws on *everything* that came before.

---

## Pre-Demo Checklist

Run through this list **before** your audience arrives:

- [ ] `aspire run` is running, all resources healthy (check the Aspire dashboard)
- [ ] Presenter dashboard accessible at the correct URL
- [ ] Display view open on projection screen / second monitor
- [ ] Attendee URL ready (QR code visible on display)
- [ ] MCP connection configured in VS Code / Copilot CLI
- [ ] Test one MCP tool call to verify connectivity (see [Test Your MCP Connection](#test-your-mcp-connection))

> ⚠️ Don't skip the MCP test. A broken MCP connection during the closer is the worst possible failure — it's the climax of your demo.

---

## Setting Up MCP for the Demo

### Option 1: VS Code with GitHub Copilot Chat

1. Ensure `.vscode/mcp.json` exists (it's included in the repo):

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

2. Update the port from the Aspire dashboard — the web endpoint port is dynamic
3. Open Copilot Chat in VS Code
4. The tools should auto-discover — you'll see them listed when you type `@`

> ⚠️ The port is dynamic. Every time you restart `aspire run`, check the Aspire dashboard for the actual web endpoint and update `mcp.json` accordingly.

### Option 2: Copilot CLI

Once `mcp.json` is configured, Copilot CLI discovers the tools automatically. No additional setup is needed.

---

## Test Your MCP Connection

Before rehearsing the full flow, verify MCP is working. Try these prompts:

| Prompt | Expected Result |
|--------|----------------|
| "What's the session status?" | Shows DOTNETAI-CONF session details via `get_session_status` |
| "How many records are in the knowledge base?" | Shows record count via `get_knowledge_stats` |
| "Search the knowledge base for Microsoft.Extensions.AI" | Returns matching results via `search_knowledge` |

If any of these fail, check the port in `mcp.json` against the Aspire dashboard and ensure the web API resource is healthy.

---

## Walking Through the Five Segments

### Segment 1: Microsoft.Extensions.AI

This is your foundation segment. Keep it clean and simple — you're setting the stage.

1. Activate the `meai` topic from the presenter dashboard
2. Navigate to the first slide
3. Generate a poll about AI experience levels
4. Launch the poll and have attendees vote
5. Close the poll and let insights generate
6. **Demo point:** Show how the `IChatClient` middleware pipeline works in `Program.cs` — this is the abstraction layer everything else builds on

> 💡 After this segment, the knowledge base has its first real-world data: poll responses and AI-generated insights. Every subsequent segment benefits from this.

### Segment 2: Knowledge Engineering

Now the snowball starts rolling. The system already has data from Segment 1.

1. Activate the `knowledge` topic
2. Show the knowledge base stats — notice the growth since Segment 1
3. Generate a poll — it's now **more contextual** because of Segment 1 data
4. **Demo point:** Use MCP to search knowledge: *"Search for what we've discussed so far"* — this calls `search_session_knowledge` and demonstrates that the knowledge base is live and growing

### Segment 3: Agentic AI

This is where the system gets visibly smarter. Agents are analyzing, curating, and synthesizing.

1. Activate the `agents` topic
2. Generate and run another poll
3. Watch the Response Analysis workflow (Analyst → Curator handoff) — narrate what's happening behind the scenes
4. **Demo point:** Show the agent activity in the presenter dashboard — the audience can see the agents working in real time

### Segment 4: MCP

Now you bring the external AI tools into the picture. This is where the audience sees the power of interoperability.

1. Activate the `mcp` topic
2. From VS Code or Copilot CLI: *"What has the audience been struggling with?"*
   - Watch it call `get_audience_questions` + `get_all_insights`
3. From VS Code or Copilot CLI: *"Search for vector database patterns"*
   - Watch it call `search_session_knowledge`
4. **Demo point:** You're now using MCP to query the live session! The AI tool has full access to everything the audience has contributed

> 💡 Pause here and let the audience absorb what just happened. An external AI tool just queried your live application's knowledge base. That's the power of MCP.

### The Closer: Full Cascade

This is the mic-drop moment. Everything comes together.

1. From VS Code or Copilot CLI, prompt:

   > *"Generate a comprehensive summary of this session including all poll results, audience questions, and key themes"*

2. This triggers `generate_session_summary` — the most complex tool in the arsenal

3. **Narrate what's happening** as the response streams back:
   - MCP server receives the request
   - Session summary workflow activates
   - Agents search the VectorData store
   - Knowledge from DataIngestion is retrieved
   - Every LLM call flows through M.E.AI abstractions
   - **One request. Five technologies. The summary streams back live.**

4. The summary appears — a comprehensive, AI-generated recap that draws on every poll, every question, every insight from the entire session

> 💡 This is your "ta-da" moment. Let the output speak for itself, then connect the dots for the audience: *"That one command just exercised every technology we covered today."*

---

## Handling Failures During a Live Demo

Things will go wrong. Here's how to recover gracefully:

| Issue | Quick Fix |
|-------|-----------|
| MCP connection fails | Check port in Aspire dashboard, update `mcp.json`, reload |
| AI response is slow | Tell the audience *"the LLM is thinking..."* — most calls complete in 10–15s |
| Poll generation fails | Use **Add Custom Poll** as a backup |
| No AI insights generated | Wait 10s, try closing/reopening the poll |
| Knowledge base empty | Ensure `aspire run` completed startup ingestion (check Aspire logs) |
| Display not updating | Check SignalR connection — refresh the display page |

> 💡 The best defense is a rehearsal. Run through this entire guide at least twice before your live demo. You'll discover your own failure modes and build muscle memory for recovery.

---

## After the Demo

When the applause dies down, here's what you've got:

- **Knowledge base stats** show growth throughout the session — quantifiable proof of the snowball effect
- **All polls, questions, and insights** are persisted in PostgreSQL — nothing is lost
- **The session can be reviewed later** — data survives app restart, so you can demo the summary again from recorded data

---

## Next Steps

- [Customize for Your Talk](03-customize-for-your-talk.md) — Adapt content for your own presentation
- [Presenter Guide](../presenter-guide.md) — Full live demo playbook
- [MCP Reference](../mcp-reference.md) — All 10 MCP tools documented
