using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Definitions;
using ConferenceAssistant.Agents.Tools;

namespace ConferenceAssistant.Agents.Workflows;

public class ResponseAnalysisWorkflow(IChatClient chatClient, AgentTools tools)
{
    public async Task<string> ExecuteAsync(string pollId)
    {
        var options = new ChatOptions
        {
            Tools = [tools.GetPollResults, tools.SearchKnowledge,
                     tools.GetAllPollResults, tools.SaveInsight]
        };

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, AgentDefinitions.ResponseAnalystInstructions),
            new(ChatRole.User, $"Analyze the results for poll ID: {pollId}. Use the tools to get the results, find context, identify trends, and generate actionable insights.")
        };

        var response = await chatClient.GetResponseAsync(messages, options);
        return response.Text ?? "Unable to analyze poll results.";
    }
}
