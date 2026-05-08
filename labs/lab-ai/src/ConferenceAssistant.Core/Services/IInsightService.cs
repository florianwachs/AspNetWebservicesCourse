using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public interface IInsightService
{
    event Action<Insight>? InsightGenerated;

    Task<Insight> AddInsightAsync(Insight insight);
    IReadOnlyList<Insight> GetInsightsForPoll(string pollId);
    IReadOnlyList<Insight> GetInsightsForTopic(string topicId);
    IReadOnlyList<Insight> GetAllInsights();
}
