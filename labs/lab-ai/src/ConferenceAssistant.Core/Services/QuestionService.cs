using System.Collections.Concurrent;
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class QuestionService : IQuestionService
{
    private readonly ISessionManager _sessionManager;
    private readonly ISessionService _sessionService;
    private readonly ConcurrentDictionary<string, byte> _wiredSessions = new();

    public event Action<AudienceQuestion>? QuestionReceived;
    public event Action<AudienceQuestion>? QuestionAnswered;
    public event Action<AudienceQuestion>? QuestionUpvoted;

    public QuestionService(ISessionManager sessionManager, ISessionService sessionService)
    {
        _sessionManager = sessionManager;
        _sessionService = sessionService;
    }

    private SessionContext GetDefaultContext()
    {
        var code = _sessionService.CurrentSession?.SessionCode
            ?? throw new InvalidOperationException("No default session loaded.");
        var ctx = _sessionManager.GetSession(code)
            ?? throw new InvalidOperationException($"Session '{code}' not found.");
        EnsureEventsWired(ctx);
        return ctx;
    }

    private void EnsureEventsWired(SessionContext ctx)
    {
        if (_wiredSessions.TryAdd(ctx.Session.SessionCode, 0))
        {
            ctx.QuestionReceived += q => QuestionReceived?.Invoke(q);
            ctx.QuestionAnswered += q => QuestionAnswered?.Invoke(q);
            ctx.QuestionUpvoted += q => QuestionUpvoted?.Invoke(q);
        }
    }

    public Task<AudienceQuestion> SubmitQuestionAsync(string text, string? topicId = null, string? attendeeId = null)
    {
        var question = GetDefaultContext().SubmitQuestion(text, topicId, attendeeId);
        return Task.FromResult(question);
    }

    public Task<AudienceQuestion?> AnswerQuestionAsync(string questionId, string answer, bool isAiGenerated = false, string authorLabel = "Presenter")
    {
        var result = GetDefaultContext().AnswerQuestion(questionId, answer, isAiGenerated, isAiGenerated ? "AI" : authorLabel);
        return Task.FromResult(result);
    }

    public Task UpvoteQuestionAsync(string questionId)
    {
        GetDefaultContext().UpvoteQuestion(questionId);
        return Task.CompletedTask;
    }

    public IReadOnlyList<AudienceQuestion> GetQuestionsForTopic(string topicId) => GetDefaultContext().GetQuestionsForTopic(topicId);
    public IReadOnlyList<AudienceQuestion> GetTopQuestions(int count = 10) => GetDefaultContext().GetTopQuestions(count);
}
