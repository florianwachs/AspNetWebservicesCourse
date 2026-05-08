using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public interface ISessionService
{
    event Action<string>? TopicActivated;
    event Action<string>? TopicCompleted;
    event Action? SessionEnded;
    event Action<Slide>? SlideChanged;

    ConferenceSession? CurrentSession { get; }
    Slide? ActiveSlide { get; }
    int ActiveSlideIndex { get; }
    int TotalSlides { get; }

    Task LoadSessionAsync(string seedTopicsPath);
    void SetDefaultSession(string sessionCode);
    Task LoadSlidesAsync(string slidesPath);
    Task StartSessionAsync();
    Task ActivateTopicAsync(string topicId);
    Task CompleteTopicAsync(string topicId);
    Task EndSessionAsync();
    SessionTopic? GetActiveTopic();
    List<Slide> GetSlidesForTopic(string topicId);
    Task AdvanceSlideAsync();
    Task GoBackSlideAsync();
    Task GoToSlideAsync(string slideId);
    Slide? GetNextSlide();
    Slide? GetPreviousSlide();
}
