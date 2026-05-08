using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class SessionService : ISessionService
{
    private readonly ISessionManager _sessionManager;
    private string? _defaultSessionCode;

    public SessionService(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    private SessionContext GetDefaultContext()
    {
        if (_defaultSessionCode is null)
            throw new InvalidOperationException("No default session loaded. Call LoadSessionAsync first.");
        return _sessionManager.GetSession(_defaultSessionCode)
            ?? throw new InvalidOperationException($"Default session '{_defaultSessionCode}' not found.");
    }

    public ConferenceSession? CurrentSession => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.Session
        : null;

    public Slide? ActiveSlide => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.ActiveSlide
        : null;

    public int ActiveSlideIndex => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.ActiveSlideIndex ?? -1
        : -1;

    public int TotalSlides => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.TotalSlides ?? 0
        : 0;

    public event Action<string>? TopicActivated;
    public event Action<string>? TopicCompleted;
    public event Action? SessionEnded;
    public event Action<Slide>? SlideChanged;

    public async Task LoadSessionAsync(string seedTopicsPath)
    {
        var json = await File.ReadAllTextAsync(seedTopicsPath);
        var context = _sessionManager.CreateSessionFromSeed(json, "0000");
        _defaultSessionCode = context.Session.SessionCode;

        // Wire context events to service events for backward compat
        context.TopicActivated += id => TopicActivated?.Invoke(id);
        context.TopicCompleted += id => TopicCompleted?.Invoke(id);
        context.SessionEnded += () => SessionEnded?.Invoke();
        context.SlideChanged += slide => SlideChanged?.Invoke(slide);
    }

    public void SetDefaultSession(string sessionCode)
    {
        var ctx = _sessionManager.GetSession(sessionCode)
            ?? throw new InvalidOperationException($"Session '{sessionCode}' not found.");
        _defaultSessionCode = sessionCode;

        ctx.TopicActivated += id => TopicActivated?.Invoke(id);
        ctx.TopicCompleted += id => TopicCompleted?.Invoke(id);
        ctx.SessionEnded += () => SessionEnded?.Invoke();
        ctx.SlideChanged += slide => SlideChanged?.Invoke(slide);
    }

    public Task LoadSlidesAsync(string slidesPath)
    {
        return _sessionManager.LoadSlidesForSessionAsync(_defaultSessionCode!, slidesPath);
    }

    public Task StartSessionAsync() { GetDefaultContext().StartSession(); return Task.CompletedTask; }
    public Task ActivateTopicAsync(string topicId) { GetDefaultContext().ActivateTopic(topicId); return Task.CompletedTask; }
    public Task CompleteTopicAsync(string topicId) { GetDefaultContext().CompleteTopic(topicId); return Task.CompletedTask; }
    public Task EndSessionAsync() { GetDefaultContext().EndSession(); return Task.CompletedTask; }

    public SessionTopic? GetActiveTopic() => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.GetActiveTopic()
        : null;

    public List<Slide> GetSlidesForTopic(string topicId) => GetDefaultContext().GetSlidesForTopic(topicId);

    public Task AdvanceSlideAsync() { GetDefaultContext().AdvanceSlide(); return Task.CompletedTask; }
    public Task GoBackSlideAsync() { GetDefaultContext().GoBackSlide(); return Task.CompletedTask; }
    public Task GoToSlideAsync(string slideId) { GetDefaultContext().GoToSlide(slideId); return Task.CompletedTask; }

    public Slide? GetNextSlide() => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.GetNextSlide()
        : null;

    public Slide? GetPreviousSlide() => _defaultSessionCode is not null
        ? _sessionManager.GetSession(_defaultSessionCode)?.GetPreviousSlide()
        : null;
}
