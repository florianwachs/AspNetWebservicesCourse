using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace TechConf.McpServer.Prompts;

[McpServerPromptType]
public static class SummarizeEventPrompt
{
    [McpServerPrompt(Name = "summarize_event")]
    public static ChatMessage Execute(
        [Description("The event ID to summarize")] string eventId)
    {
        return new ChatMessage(ChatRole.User,
            $"""
            Please summarize the TechConf event with ID {eventId}.
            Include: title, dates, location, number of sessions,
            key speakers, and registration status.
            Use the available tools to gather this information.
            """);
    }
}
