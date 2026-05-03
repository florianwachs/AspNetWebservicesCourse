namespace ConferenceAssistant.Core.Services;

public interface IInsightGenerationService
{
    Task GenerateTopicInsightsAsync(string topicId, string sessionCode);
    Task GeneratePollInsightsAsync(string pollId, string sessionCode);
    Task GenerateQuestionInsightsAsync(string topicId, string sessionCode);
}
