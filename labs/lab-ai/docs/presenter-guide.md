# Presenter Guide — Running a Live Demo

## Who This Is For

You're a speaker about to use Conference Pulse for a live presentation. This guide covers everything from pre-show setup to post-session review.

## Pre-Show Checklist (Day Before)

- [ ] Azure OpenAI resource is accessible and not rate-limited
- [ ] All user secrets configured in the AppHost project
- [ ] `aspire run` starts cleanly — all resources healthy
- [ ] Default demo session loads with your customized topics
- [ ] Slides display correctly on `/display/{code}`
- [ ] Knowledge base shows records ingested (check stats in presenter dashboard)
- [ ] Test poll generation for each topic
- [ ] Test Q&A with AI answers
- [ ] MCP connection works from your preferred tool
- [ ] Backup plan ready (see [When Things Go Wrong](#when-things-go-wrong) below)

## Pre-Show Setup (30 Minutes Before)

1. Start Docker Desktop
2. Run `az login` to refresh Azure credentials
3. `aspire run` and wait for all resources to show "Running"
4. Open presenter dashboard — enter PIN
5. Open display view on the projection screen/external monitor
6. Verify attendee URL (QR code) is accessible via dev tunnel
7. Test one quick poll generation to warm up the AI
8. Clear any test data from your practice runs (create a fresh session or use the default)

## Screen Layout

Recommend a 3-screen or 2-screen setup:

### 3-Screen Setup (Ideal)

| Screen | Content | Audience Can See? |
|--------|---------|-------------------|
| Laptop | Presenter dashboard (`/presenter/{code}`) | No |
| External Monitor | Display view (`/display/{code}`) | Yes (projection) |
| Phone/Tablet | Attendee view (`/session/{code}`) for testing | No |

### 2-Screen Setup

| Screen | Content | Audience Can See? |
|--------|---------|-------------------|
| Laptop (split) | Presenter dashboard + VS Code for MCP | No |
| External Monitor | Display view | Yes (projection) |

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `→` or `Space` | Next slide |
| `←` | Previous slide |
| `P` | Toggle presenter notes |
| `Esc` | Exit slide focus |

## Pacing Guide

For a 60-minute session with 5 topics:

| Segment | Duration | Key Actions |
|---------|----------|-------------|
| Intro & Setup | 5 min | Welcome, show QR code, explain the concept |
| Topic 1 (M.E.AI) | 10 min | Activate topic, generate poll, vote, show results |
| Topic 2 (Knowledge) | 12 min | Show KB growth, generate contextual poll, demo semantic search |
| Topic 3 (Agents) | 12 min | Show agent workflows, demonstrate handoff pattern |
| Topic 4 (MCP) | 10 min | Connect Copilot CLI, call MCP tools live, show external data |
| The Closer | 8 min | Generate full summary via MCP, narrate the cascade |
| Q&A / Wrap-up | 3 min | Review audience questions, final thoughts |

## Running the Session

### Opening (First 5 Minutes)

1. Have the display showing a title slide
2. Welcome the audience
3. "Take out your phones and scan the QR code" — point to the display
4. "This app IS the presentation. Everything you see is generated live."
5. Go Live on the presenter dashboard

### During Each Topic

1. **Activate the topic** — Display navigates to the topic's first slide
2. **Walk through slides** — Use keyboard shortcuts for pacing
3. **Generate a poll** — "Let's see what you think..." Wait for AI generation (~5-10s)
4. **Launch the poll** — "Vote now!" Watch real-time results on display
5. **Close and analyze** — "Let's see what the AI thinks about your responses..."
6. **Show insights** — Read the AI-generated insight aloud
7. **Take questions** — "Type your questions in the app" — show AI answers appearing

### The MCP Closer

This is the demonstration climax:

1. Switch to VS Code or terminal (visible on projection)
2. Open Copilot CLI or Copilot Chat
3. Say: "Now I'm going to ask our AI to summarize everything we've done"
4. Type: "Generate a comprehensive summary of this session including all poll results and audience questions"
5. Narrate as the summary generates:
   - "MCP receives the request..."
   - "The agent searches our knowledge base..."
   - "Remember, that knowledge base has everything — your poll responses, your questions, the session outline..."
6. Let the summary complete
7. "And THAT is the Microsoft AI stack for .NET."

## Audience Interaction Tips

- **Encourage voting:** "I need at least 20 votes before I close this poll"
- **Reference results:** "Interesting — 65% of you have used dependency injection but only 20% have used IChatClient"
- **Acknowledge questions:** "Great question from the audience — look at the AI's answer appearing right now"
- **Build narrative:** "Notice how the polls are getting smarter? That's the snowball effect — the AI has your previous responses as context now"

## When Things Go Wrong

### AI Is Slow

- Stay calm — "The LLM is processing a lot of context"
- Talk about what's happening behind the scenes while waiting
- If it takes >20 seconds, move on and come back

### Poll Generation Fails

- Use "Add Custom Poll" to create one manually
- Have 2-3 backup poll questions written down for each topic

### MCP Connection Fails

- Check the port (it's dynamic — may have changed from practice)
- Have the correct URL ready to paste into mcp.json
- Backup: Use the presenter dashboard's built-in summary feature

### Display Not Updating

- Refresh the display browser tab
- Check that the presenter dashboard is connected (green indicator)

### Attendees Can't Join

- Verify the dev tunnel is running (check Aspire dashboard)
- Share the URL manually if QR code isn't working
- Have the URL in your slide notes for easy reference

### Internet Goes Down

- Core features work locally (manual polls, slides, voting on local network)
- Azure OpenAI calls will fail — switch to manual mode
- "We've just demonstrated what graceful degradation looks like!"

## Post-Session

1. The session data persists in PostgreSQL — you can review later
2. Use MCP to generate a final summary if you didn't during the talk
3. Share the attendee URL for a short time after the session for Q&A follow-up
4. Check PgWeb (from Aspire dashboard) to explore the stored data
5. Stop the app: `Ctrl+C` or `aspire stop`

## Customizing for Your Talk

See [Tutorial: Customize for Your Talk](tutorials/03-customize-for-your-talk.md) for adapting the content.
