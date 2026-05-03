using System.Collections.Concurrent;
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class PollService : IPollService
{
    private readonly ISessionManager _sessionManager;
    private readonly ISessionService _sessionService;
    private readonly ConcurrentDictionary<string, byte> _wiredSessions = new();

    public event Action<Poll>? PollActivated;
    public event Action<Poll>? PollClosed;
    public event Action<PollResponse>? ResponseReceived;

    public PollService(ISessionManager sessionManager, ISessionService sessionService)
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
            ctx.PollActivated += p => PollActivated?.Invoke(p);
            ctx.PollClosed += p => PollClosed?.Invoke(p);
            ctx.ResponseReceived += r => ResponseReceived?.Invoke(r);
        }
    }

    public Task<Poll> CreatePollAsync(string? topicId, string question, List<string> options, PollSource source = PollSource.Generated, bool allowOther = false)
    {
        var poll = GetDefaultContext().CreatePoll(topicId, question, options, source, allowOther);
        return Task.FromResult(poll);
    }

    public Task ActivatePollAsync(string pollId)
    {
        GetDefaultContext().ActivatePoll(pollId);
        return Task.CompletedTask;
    }

    public Task ClosePollAsync(string pollId)
    {
        GetDefaultContext().ClosePoll(pollId);
        return Task.CompletedTask;
    }

    public Task<PollResponse> SubmitResponseAsync(string pollId, string selectedOption, string? attendeeId = null, string? otherText = null)
    {
        var response = GetDefaultContext().SubmitResponse(pollId, selectedOption, attendeeId, otherText);
        return Task.FromResult(response);
    }

    public Poll? GetActivePoll() => GetDefaultContext().GetActivePoll();
    public Poll? GetPoll(string pollId) => GetDefaultContext().GetPoll(pollId);
    public IReadOnlyList<Poll> GetPollsForTopic(string topicId) => GetDefaultContext().GetPollsForTopic(topicId);
    public IReadOnlyList<PollResponse> GetResponsesForPoll(string pollId) => GetDefaultContext().GetResponsesForPoll(pollId);
    public Dictionary<string, int> GetPollResults(string pollId) => GetDefaultContext().GetPollResults(pollId);
    public List<string> GetOtherResponses(string pollId) => GetDefaultContext().GetOtherResponses(pollId);
}
