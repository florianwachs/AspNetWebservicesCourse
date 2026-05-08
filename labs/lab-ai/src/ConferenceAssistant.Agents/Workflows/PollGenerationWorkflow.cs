using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Definitions;
using ConferenceAssistant.Agents.Tools;

namespace ConferenceAssistant.Agents.Workflows;

public class PollGenerationWorkflow(IChatClient chatClient, AgentTools tools)
{
    public async Task<string> ExecuteAsync(string topicId)
    {
        var options = new ChatOptions
        {
            Tools = [tools.GetCurrentTopic, tools.SearchKnowledge, tools.GetAudienceQuestions,
                     tools.GetAllPollResults, tools.GetAllInsights, tools.CreatePoll]
        };

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, AgentDefinitions.SurveyArchitectInstructions),
            new(ChatRole.User, $"Generate an engaging poll for the topic currently being discussed. The topic ID is: {topicId}. Use the available tools to understand the context and create a relevant poll.")
        };

        var response = await chatClient.GetResponseAsync(messages, options);
        return response.Text ?? "Unable to generate poll.";
    }
}
