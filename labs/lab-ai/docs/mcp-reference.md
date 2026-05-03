# MCP Tools Reference

## Overview

Conference Pulse exposes an MCP (Model Context Protocol) server at `/mcp` via Streamable HTTP. Any MCP-compatible client (VS Code, GitHub Copilot, Claude Desktop, custom apps) can connect and use the tools listed below.

The server registers **10 tools** across two categories — conference tools and knowledge tools — plus MCP resources for direct data access.

---

## Connecting to the MCP Server

### VS Code / GitHub Copilot Chat

Add to `.vscode/mcp.json`:

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

> ⚠️ The port is dynamic — check the Aspire dashboard for the actual web endpoint.

### Copilot CLI

Once configured in `.vscode/mcp.json`, Copilot CLI automatically discovers the tools. No additional setup is required.

### Other MCP Clients

Any client that supports Streamable HTTP transport can connect by pointing to `http://<host>:<port>/mcp`.

---

## Tools Reference

### Conference Tools

Defined in `src/ConferenceAssistant.Mcp/Tools/ConferenceTools.cs`.

#### 1. `get_session_status`

Returns the current conference session status including title, status, active topic, and all topics with their statuses.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| *(none)* | — | — | — |

**Returns:** Markdown-formatted session overview with a topic list showing status markers:
- `▶` active
- `✓` completed
- `○` pending

**Example prompt:**
> "What's the current status of the conference session?"

---

#### 2. `get_active_poll`

Returns the currently active poll with its question, options, and current vote counts.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| *(none)* | — | — | — |

**Returns:** Markdown with poll question, total votes, and per-option counts with percentages.

**Example prompt:**
> "What poll is running right now?"

---

#### 3. `get_poll_results`

Returns detailed results for a specific poll including question, options, vote counts, and percentages.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `pollId` | `string` | ✅ | The unique identifier of the poll |

**Returns:** Markdown with poll ID, total responses, and bar-chart-style results ordered by votes.

**Example prompt:**
> "Show me the results for poll abc123"

---

#### 4. `search_session_knowledge`

Searches the session knowledge base (vector store) for content relevant to the query.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `query` | `string` | ✅ | The search query |
| `maxResults` | `int` | ❌ (default: `5`) | Maximum number of results to return |

**Returns:** Markdown with numbered results showing source, content, and context.

**Example prompt:**
> "Search the knowledge base for information about IChatClient"

---

#### 5. `get_audience_questions`

Returns the top audience questions ordered by upvotes.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `count` | `int` | ❌ (default: `10`) | Number of top questions to return |

**Returns:** Markdown with numbered questions showing text, upvotes, timestamp, and any answers (AI or human).

**Example prompt:**
> "What are the top audience questions?"

---

#### 6. `get_topic_insights`

Returns all AI-generated insights for a specific topic.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `topicId` | `string` | ✅ | The topic identifier |

**Returns:** Markdown with insights grouped by type with timestamps.

**Available topic IDs:** `meai`, `knowledge`, `agents`, `mcp`, `closer`

**Example prompt:**
> "What insights have been generated for the agents topic?"

---

#### 7. `get_all_insights`

Returns all AI-generated insights from the entire session.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| *(none)* | — | — | — |

**Returns:** Markdown with insights grouped by type (e.g., Poll Analysis, Topic Summary, Knowledge Gap).

**Example prompt:**
> "Show me all the insights from this session"

---

#### 8. `generate_session_summary`

Generates a comprehensive summary of the entire conference session including all polls, insights, audience questions, and knowledge base statistics.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| *(none)* | — | — | — |

**Returns:** Extensive markdown with:
- Session overview
- Topics covered (with polls and insights per topic)
- All insights summary
- Audience questions
- Knowledge base stats
- Session statistics

**Example prompt:**
> "Generate a complete summary of today's session"

---

### Knowledge Tools

Defined in `src/ConferenceAssistant.Mcp/Tools/KnowledgeTools.cs`.

#### 9. `search_knowledge`

Searches the conference knowledge base (vector store) for content matching the query.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `query` | `string` | ✅ | The search query |

**Returns:** Markdown with numbered results showing source and content.

**Example prompt:**
> "Find knowledge base entries about vector stores"

---

#### 10. `get_knowledge_stats`

Returns statistics about the knowledge base including total record count.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| *(none)* | — | — | — |

**Returns:** Markdown with total record count.

**Example prompt:**
> "How many records are in the knowledge base?"

---

## MCP Resources

The server also exposes MCP resources (defined in `ConferenceResources.cs`) for direct data access. MCP resources allow clients to read structured data from the server without invoking a tool — useful for populating context or displaying live state.

**Source:** `src/ConferenceAssistant.Mcp/Server/ConferenceResources.cs`

---

## MCP Client Connections

Conference Pulse also acts as an MCP **client**, connecting to external servers to enrich the local knowledge base.

| External Server | Purpose | Used By |
|-----------------|---------|---------|
| Microsoft Learn | Official .NET/Azure documentation | Knowledge Curator agent |
| DeepWiki | GitHub repository knowledge | Knowledge Curator agent |

When the Knowledge Curator fetches content from these sources, it is ingested into the local vector store — making it available for future queries via `search_knowledge` and `search_session_knowledge`.

**Source:** `src/ConferenceAssistant.Mcp/Clients/McpContentClient.cs`

---

## Example Interactions

Below are example prompts and the tools they would trigger:

| Prompt | Tools Triggered |
|--------|----------------|
| "Summarize this session including all poll results and key themes" | `generate_session_summary` |
| "What has the audience been struggling with?" | `get_audience_questions` + `get_all_insights` |
| "Search for what we discussed about semantic search" | `search_session_knowledge` (query: `"semantic search"`) |
| "What's happening right now in the session?" | `get_session_status` + `get_active_poll` |

---

## Source Files

| File | Contents |
|------|----------|
| `src/ConferenceAssistant.Mcp/Tools/ConferenceTools.cs` | 8 conference tools |
| `src/ConferenceAssistant.Mcp/Tools/KnowledgeTools.cs` | 2 knowledge tools |
| `src/ConferenceAssistant.Mcp/Server/ConferenceResources.cs` | MCP resources |
| `src/ConferenceAssistant.Mcp/Clients/McpContentClient.cs` | External MCP client |
