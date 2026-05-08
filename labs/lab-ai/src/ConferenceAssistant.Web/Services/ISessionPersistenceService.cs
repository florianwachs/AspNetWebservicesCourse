using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Web.Services;

public interface ISessionPersistenceService
{
    Task SaveSessionAsync(SessionContext context);
    Task<ConferenceSession?> LoadSessionAsync(string sessionCode);
    Task<PersistedSessionData?> LoadSessionRuntimeDataAsync(string sessionId);
    Task SavePollAsync(string sessionId, Poll poll);
    Task SavePollResponseAsync(PollResponse response);
    Task SaveQuestionAsync(string sessionId, AudienceQuestion question);
    Task SaveQuestionAnswerAsync(string questionId, QuestionAnswer answer);
    Task SaveInsightAsync(string sessionId, Insight insight);
    Task ClearRuntimeDataAsync(string sessionId);
    Task DeleteSessionAsync(string sessionId);
    Task<List<ConferenceSession>> LoadAllSessionsAsync();
}

public record PersistedSessionData(
    List<Poll> Polls,
    List<PollResponse> Responses,
    List<AudienceQuestion> Questions,
    List<Insight> Insights,
    List<Slide> Slides);
