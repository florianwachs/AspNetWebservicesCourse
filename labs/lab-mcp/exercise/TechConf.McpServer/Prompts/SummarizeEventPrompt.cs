using System.ComponentModel;
using ModelContextProtocol.Server;

namespace TechConf.McpServer.Prompts;

// TODO: Task 5 — Implement the summarize_event prompt
//
// This prompt provides a reusable template for AI assistants to summarize events.
//
// Requirements:
// 1. Decorate the class with [McpServerPromptType]
// 2. Create a static method with [McpServerPrompt(Name = "summarize_event")]
// 3. Parameter: string eventId with [Description("The event ID to summarize")]
// 4. Return a ChatMessage with ChatRole.User containing instructions like:
//    "Please summarize the TechConf event with ID {eventId}.
//     Include: title, dates, location, number of sessions,
//     key speakers, and registration status.
//     Use the available tools to gather this information."
//
// Note: Import the correct ChatMessage and ChatRole types from
// ModelContextProtocol or Microsoft.Extensions.AI

public static class SummarizeEventPrompt
{
    // Implement the prompt method here
}
