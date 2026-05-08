# The Microsoft AI Stack for .NET — A Living Presentation

## Introduction: No Slides, Just Code

Welcome to a session unlike any you've attended. There are no slides. This application IS the presentation. Everything you see — polls, insights, visualizations — is generated in real-time by AI agents powered by the Microsoft AI stack for .NET.

Take out your phones and scan the QR code to join. You'll be voting on polls, asking questions, and shaping the content of this session live.

## Segment 1: Microsoft.Extensions.AI — The Common Language

The foundation of everything in the Microsoft AI stack is `Microsoft.Extensions.AI` (M.E.AI). It provides provider-agnostic abstractions that let you swap LLM providers without changing your application code.

### Key Abstractions

- **IChatClient** — A unified interface for chat completions. Works with Azure OpenAI, OpenAI, Ollama, and any compatible provider. Your code talks to `IChatClient`, not to a specific SDK.
- **IEmbeddingGenerator<string, Embedding<float>>** — Generate text embeddings for semantic search. Same provider-agnostic pattern.
- **ChatClientBuilder** — Composable middleware pipeline. Add function invocation, OpenTelemetry tracing, rate limiting, and caching as chainable middleware.
- **AIFunctionFactory.Create()** — Turn any .NET method into a tool that AI models can call. The bridge between AI reasoning and your code.

### Why It Matters

Before M.E.AI, every AI provider had its own SDK with different patterns. If you wanted to switch from OpenAI to Azure OpenAI to a local Ollama model, you rewrote your integration layer. M.E.AI eliminates that. One interface. Any provider. Middleware for cross-cutting concerns.

## Segment 2: Knowledge Engineering — DataIngestion + VectorData

AI without knowledge is just pattern matching. This segment covers how to turn raw content into searchable, semantic knowledge.

### Microsoft.Extensions.DataIngestion

A 5-stage pipeline for transforming documents into vector store entries:

1. **Reader** — Reads source documents. `MarkdigReader` for markdown, custom readers for other formats.
2. **DocumentProcessors** — Pre-processing transforms on the full document.
3. **Chunker** — Splits documents into meaningful chunks. `HeaderChunker` splits on headings, `DocumentTokenChunker` splits by token count.
4. **ChunkProcessors** — Enrichment transforms on each chunk. Add summaries, keywords, sentiment analysis, classifications.
5. **Writer** — Persists enriched chunks to a destination. `VectorStoreWriter` writes to any VectorData store.

### Microsoft.Extensions.VectorData

The storage and retrieval layer for semantic search:

- **InMemoryVectorStore** — Zero-infrastructure vector store. Perfect for demos and prototypes.
- **VectorStoreCollection<TKey, TRecord>** — Typed collection with CRUD + search operations.
- **SearchAsync()** — Semantic search with configurable top-K and filters.
- **Auto-embedding** — Configures the store to automatically generate embeddings from text fields.

### The Living Knowledge Base

In this session, the knowledge base grows live. The session outline was ingested at startup. Your poll responses get ingested after each segment. By the end, the agents have context from everything we've discussed AND everything you've said.

## Segment 3: Agentic AI — The Microsoft Agent Framework

Agents are AI models with tools, memory, and the ability to take action. The Microsoft Agent Framework provides the building blocks.

### ChatClientAgent

The core agent type. Built on top of `IChatClient`, it adds:

- **System instructions** — Define the agent's persona and behavior
- **Tools** — Functions the agent can call (via AIFunctionFactory)
- **Structured output** — Type-safe responses
- **Conversation history** — Multi-turn context

### Agent Workflows with AgentWorkflowBuilder

Orchestrate multiple agents working together:

- **Sequential** — Agents execute in order, each building on previous output
- **Handoff** — One agent passes control to another based on conditions
- **Group Chat** — Multiple agents collaborate in a moderated discussion

### Our Agents

This session uses three specialized agents:

1. **Survey Architect** — Generates contextual poll questions using the knowledge base and audience history
2. **Response Analyst** — Interprets poll results, identifies trends, and generates insights
3. **Knowledge Curator** — Manages the knowledge base, enriches content, and retrieves relevant information

### The Snowball Effect

Each segment makes the agents smarter. Segment 1 polls are generic. By Segment 3, the agents have context from the outline, previous responses, and accumulated insights. The polls get more targeted. The analysis gets deeper. This is what a growing knowledge base enables.

## Segment 4: Interoperability — Model Context Protocol (MCP)

MCP is an open protocol that lets AI applications discover and use tools from external servers. It's like a universal adapter for AI capabilities.

### MCP Server: Exposing Our Tools

This application exposes its capabilities as an MCP server:

- `get_active_polls` — Returns current polls
- `get_poll_results` — Returns results for a specific poll
- `search_session_knowledge` — Semantic search over the knowledge base
- `get_session_insights` — Returns all generated insights
- `get_audience_questions` — Returns audience questions
- `generate_session_summary` — Triggers the full summary workflow

Any MCP-compatible client — GitHub Copilot, VS Code, Claude, custom apps — can connect to this server and use these tools.

### MCP Client: Consuming External Knowledge

The application also acts as an MCP client, connecting to external servers to fetch relevant documentation and enrich the knowledge base in real-time.

### Why MCP Matters

MCP creates an ecosystem. Your AI application's capabilities become composable building blocks that any other AI application can use. No custom integrations. No API wrappers. Just a protocol.

## Segment 5: The Closer — Full Stack in One Command

Every technology we've discussed comes together in one moment.

We connect GitHub Copilot to our MCP server. One request: "Summarize this session." Watch the cascade:

1. **Copilot** sends the request to our MCP server
2. **MCP server** discovers the `generate_session_summary` tool
3. **Agent Framework** orchestrates the Summary Workflow — Survey Architect, Response Analyst, and Knowledge Curator collaborate in a group chat
4. **VectorData** searches all accumulated knowledge — outline chunks, poll responses, audience questions, external docs
5. **DataIngestion** — all that searchable content was put there by the ingestion pipelines running throughout the session
6. **M.E.AI** — every LLM call, every embedding, every tool invocation flows through the provider-agnostic abstractions

One command. Five technologies. A complete AI application built live in 60 minutes.

## Resources and Next Steps

- Microsoft.Extensions.AI: https://github.com/dotnet/extensions
- Microsoft Agent Framework: https://github.com/microsoft/agent-framework
- Model Context Protocol for .NET: https://github.com/modelcontextprotocol/csharp-sdk
- Microsoft.Extensions.VectorData: https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases
- This application's source: [Repository URL]
