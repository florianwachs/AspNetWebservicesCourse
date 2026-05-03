# Lab: Building an MCP Server for AI Integration

## Overview

In this lab, you will build a **Model Context Protocol (MCP)** server in C# that exposes TechConf event management data to AI assistants. MCP is an open standard that enables AI models to discover and invoke tools, query resources, and use prompt templates provided by external servers. By the end of this lab your MCP server will let any MCP-compatible AI assistant search events, create new events, register attendees, and summarize event details — all through a well-defined protocol.

## Prerequisites

- .NET 10 SDK installed
- Basic C# and Entity Framework Core knowledge
- Node.js installed (required for the MCP Inspector testing tool)

## Learning Objectives

1. Understand the Model Context Protocol and its role in AI integration
2. Configure a .NET Generic Host as an MCP server with stdio transport
3. Implement MCP tools that perform CRUD operations against a database
4. Use `[Description]` attributes to provide rich metadata for AI tool discovery
5. Create reusable MCP prompt templates that guide AI behavior
6. Test and debug MCP servers using the MCP Inspector

## Getting Started

```bash
cd labs/lab-mcp/exercise/TechConf.McpServer
dotnet run
```

The server starts and communicates via stdin/stdout using the MCP protocol. To interact with it visually, use the MCP Inspector (see Task 6).

## Tasks

### Task 1: Set Up the MCP Server

Open `Program.cs`. The database context is already registered. Your job is to configure the MCP server.

**Add the following to the service registration:**

```csharp
builder.Services
    .AddMcpServer()                  // Register the MCP server
    .WithStdioServerTransport()      // Use stdin/stdout communication
    .WithToolsFromAssembly()         // Auto-discover [McpServerTool] classes
    .WithPromptsFromAssembly();      // Auto-discover [McpServerPrompt] classes
```

> 💡 The `ModelContextProtocol` NuGet package is already referenced in the project file.

### Task 2: Implement the `search_events` Tool

Open `Tools/SearchEventsTool.cs`. Implement a tool that searches events by keyword.

**Requirements:**
- Decorate the class with `[McpServerToolType]`
- Create a static async method decorated with `[McpServerTool(Name = "search_events")]` and `[Description("Search TechConf events by keyword in titles and descriptions")]`
- Accept parameters: `AppDbContext db`, `string query`, `int maxResults = 10`, `CancellationToken ct`
- Add `[Description(...)]` to each parameter for AI discoverability
- Filter events where Title or Description contains the query string
- Return JSON-serialized results or a friendly "no results" message

### Task 3: Implement the `create_event` Tool

Open `Tools/CreateEventTool.cs`. Implement a tool that creates new events.

**Requirements:**
- Validate that the title is not empty and the end date is after the start date
- Create the event with `Status = Draft`
- Return a success message including the new event ID

### Task 4: Implement the `register_attendee` Tool

Open `Tools/RegisterAttendeeTool.cs`. Implement a tool that registers attendees for events.

**Requirements:**
- Find the event and include its registrations
- Check capacity: return an error if the event is full
- Check duplicates: return a warning if the email is already registered
- Create or reuse the attendee record, add a registration, and save

### Task 5: Add the `summarize_event` Prompt

Open `Prompts/SummarizeEventPrompt.cs`. Create a reusable prompt template.

**Requirements:**
- Decorate the class with `[McpServerPromptType]` and the method with `[McpServerPrompt(Name = "summarize_event")]`
- Accept an `eventId` parameter with a `[Description]`
- Return a `ChatMessage` with `ChatRole.User` containing instructions for the AI to summarize the event using the available tools

### Task 6: Test with MCP Inspector

Run the MCP Inspector to visually test your server:

```bash
cd labs/lab-mcp/exercise/TechConf.McpServer
npx @modelcontextprotocol/inspector dotnet run
```

**Verify:**
1. Open the Inspector URL shown in the terminal
2. Click **Tools** → you should see `search_events`, `create_event`, `register_attendee`
3. Click **Prompts** → you should see `summarize_event`
4. Test `search_events` with query `"Conf"` — should return seed data events
5. Test `create_event` with sample data — should return a success message
6. Test `register_attendee` with an existing event ID — should register successfully

## Stretch Goals

1. **SSE/HTTP Transport** — Add an HTTP-based transport alongside stdio so the server can also be accessed over the network
2. **Event List Resource** — Add an MCP resource that exposes the full event list as structured data
3. **`find_sessions_by_topic` Prompt** — Create a prompt template that instructs the AI to find sessions matching a given topic keyword

## Solution

A complete working solution is available in the `solution/` directory:

```bash
cd labs/lab-mcp/solution/TechConf.McpServer
dotnet run
```
