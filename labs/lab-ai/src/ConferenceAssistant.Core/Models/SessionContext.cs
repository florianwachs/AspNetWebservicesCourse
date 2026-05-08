using System.Collections.Concurrent;
using ConferenceAssistant.Core.Services;

namespace ConferenceAssistant.Core.Models;

public class SessionContext
{
    private readonly object _lock = new();
    private int _activeSlideIndex = -1;

    public ConferenceSession Session { get; }
    public List<Slide> AllSlides { get; private set; } = [];

    // Per-session collections
    private readonly ConcurrentDictionary<string, Poll> _polls = new();
    private readonly ConcurrentBag<PollResponse> _responses = new();
    private readonly ConcurrentDictionary<string, AudienceQuestion> _questions = new();
    private readonly ConcurrentBag<Insight> _insights = new();

    // Per-session events
    public event Action<string>? TopicActivated;
    public event Action<string>? TopicCompleted;
    public event Action? SessionEnded;
    public event Action<Slide>? SlideChanged;
    public event Action<Poll>? PollActivated;
    public event Action<Poll>? PollClosed;
    public event Action<PollResponse>? ResponseReceived;
    public event Action<AudienceQuestion>? QuestionReceived;
    public event Action<AudienceQuestion>? QuestionAnswered;
    public event Action<AudienceQuestion>? QuestionUpvoted;
    public event Action<AudienceQuestion>? QuestionRemoved;
    public event Action<AudienceQuestion>? QuestionModerated;
    public event Action<Insight>? InsightGenerated;
    public event Action<bool>? SlidesVisibilityChanged;
    public event Action<AudienceQuestion?>? QuestionSpotlighted;

    public SessionContext(ConferenceSession session)
    {
        Session = session;
    }

    // --- Slide State ---
    public Slide? ActiveSlide => _activeSlideIndex >= 0 && _activeSlideIndex < AllSlides.Count
        ? AllSlides[_activeSlideIndex] : null;
    public int ActiveSlideIndex => _activeSlideIndex;
    public int TotalSlides => AllSlides.Count;
    public bool SlidesHidden { get; private set; }

    public void SetSlidesVisibility(bool visible)
    {
        SlidesHidden = !visible;
        SlidesVisibilityChanged?.Invoke(!visible);
    }

    public void LoadSlides(List<Slide> slides)
    {
        AllSlides = slides;
        foreach (var topic in Session.Topics)
        {
            topic.Slides = AllSlides.Where(s => s.TopicId == topic.Id).ToList();
        }
    }

    // --- Session Lifecycle ---
    public void StartSession()
    {
        lock (_lock)
        {
            Session.Status = SessionStatus.Live;
            Session.StartedAt = DateTimeOffset.UtcNow;
        }
    }

    public void EndSession()
    {
        lock (_lock)
        {
            Session.Status = SessionStatus.Completed;
            Session.EndedAt = DateTimeOffset.UtcNow;
            foreach (var topic in Session.Topics.Where(t => t.Status == TopicStatus.Active))
                topic.Status = TopicStatus.Completed;
            Session.ActiveTopicId = null;
        }
        SessionEnded?.Invoke();
    }

    // --- Topic Navigation ---
    public void ActivateTopic(string topicId)
    {
        lock (_lock)
        {
            var currentActive = Session.Topics.FirstOrDefault(t => t.Status == TopicStatus.Active);
            if (currentActive is not null)
                currentActive.Status = TopicStatus.Completed;

            var topic = Session.Topics.FirstOrDefault(t => t.Id == topicId)
                ?? throw new ArgumentException($"Topic '{topicId}' not found.");
            topic.Status = TopicStatus.Active;
            Session.ActiveTopicId = topicId;
        }

        TopicActivated?.Invoke(topicId);

        // Auto-navigate to first slide of this topic
        var firstSlideIndex = AllSlides.FindIndex(s => s.TopicId == topicId);
        if (firstSlideIndex >= 0)
        {
            _activeSlideIndex = firstSlideIndex;
            SlideChanged?.Invoke(AllSlides[_activeSlideIndex]);
        }
    }

    public void CompleteTopic(string topicId)
    {
        lock (_lock)
        {
            var topic = Session.Topics.FirstOrDefault(t => t.Id == topicId)
                ?? throw new ArgumentException($"Topic '{topicId}' not found.");
            topic.Status = TopicStatus.Completed;
            if (Session.ActiveTopicId == topicId)
                Session.ActiveTopicId = null;
        }
        TopicCompleted?.Invoke(topicId);
    }

    public SessionTopic? GetActiveTopic()
        => Session.Topics.FirstOrDefault(t => t.Status == TopicStatus.Active);

    // --- Slide Navigation ---
    public void AdvanceSlide()
    {
        if (AllSlides.Count == 0) return;
        var newIndex = _activeSlideIndex + 1;
        if (newIndex >= AllSlides.Count) return;
        _activeSlideIndex = newIndex;
        SlideChanged?.Invoke(AllSlides[_activeSlideIndex]);
        SyncTopicToSlide();
    }

    public void GoBackSlide()
    {
        if (AllSlides.Count == 0) return;
        var newIndex = _activeSlideIndex - 1;
        if (newIndex < 0) return;
        _activeSlideIndex = newIndex;
        SlideChanged?.Invoke(AllSlides[_activeSlideIndex]);
        SyncTopicToSlide();
    }

    public void GoToSlide(string slideId)
    {
        var index = AllSlides.FindIndex(s => s.Id == slideId);
        if (index < 0) return;
        _activeSlideIndex = index;
        SlideChanged?.Invoke(AllSlides[_activeSlideIndex]);
        SyncTopicToSlide();
    }

    public void GoToSlideIndex(int index)
    {
        if (index < 0 || index >= AllSlides.Count) return;
        _activeSlideIndex = index;
        SlideChanged?.Invoke(AllSlides[_activeSlideIndex]);
        SyncTopicToSlide();
    }

    public Slide? GetNextSlide()
    {
        var nextIndex = _activeSlideIndex + 1;
        return nextIndex < AllSlides.Count ? AllSlides[nextIndex] : null;
    }

    public Slide? GetPreviousSlide()
    {
        var prevIndex = _activeSlideIndex - 1;
        return prevIndex >= 0 ? AllSlides[prevIndex] : null;
    }

    public List<Slide> GetSlidesForTopic(string topicId)
        => AllSlides.Where(s => s.TopicId == topicId).ToList();

    // --- Poll Management ---
    public Poll CreatePoll(string? topicId, string question, List<string> options,
        PollSource source = PollSource.Generated, bool allowOther = false)
    {
        var poll = new Poll
        {
            TopicId = topicId,
            Question = question,
            Options = options,
            AllowOther = allowOther,
            Source = source,
            Status = PollStatus.Draft
        };
        _polls[poll.Id] = poll;
        return poll;
    }

    public void ActivatePoll(string pollId)
    {
        if (!_polls.TryGetValue(pollId, out var poll))
            throw new ArgumentException($"Poll '{pollId}' not found.");
        // Close any currently active poll
        foreach (var p in _polls.Values.Where(p => p.Status == PollStatus.Active))
        {
            p.Status = PollStatus.Closed;
            p.ClosedAt = DateTimeOffset.UtcNow;
            PollClosed?.Invoke(p);
        }
        poll.Status = PollStatus.Active;
        PollActivated?.Invoke(poll);
    }

    public void ClosePoll(string pollId)
    {
        if (!_polls.TryGetValue(pollId, out var poll))
            throw new ArgumentException($"Poll '{pollId}' not found.");
        poll.Status = PollStatus.Closed;
        poll.ClosedAt = DateTimeOffset.UtcNow;
        PollClosed?.Invoke(poll);
    }

    public PollResponse SubmitResponse(string pollId, string selectedOption, string? attendeeId = null, string? otherText = null)
    {
        if (!_polls.TryGetValue(pollId, out var poll))
            throw new ArgumentException($"Poll '{pollId}' not found.");
        var response = new PollResponse
        {
            PollId = pollId,
            SelectedOption = selectedOption,
            OtherText = selectedOption == "Other" ? otherText : null,
            AttendeeId = attendeeId
        };
        _responses.Add(response);
        ResponseReceived?.Invoke(response);
        return response;
    }

    public Poll? GetActivePoll()
        => _polls.Values.FirstOrDefault(p => p.Status == PollStatus.Active);

    public Poll? GetPoll(string pollId)
        => _polls.TryGetValue(pollId, out var poll) ? poll : null;

    public IReadOnlyList<Poll> GetPollsForTopic(string topicId)
        => _polls.Values.Where(p => p.TopicId == topicId).ToList();

    public IReadOnlyList<PollResponse> GetResponsesForPoll(string pollId)
        => _responses.Where(r => r.PollId == pollId).ToList();

    public Dictionary<string, int> GetPollResults(string pollId)
    {
        if (!_polls.TryGetValue(pollId, out var poll)) return new();
        var results = poll.Options.ToDictionary(o => o, _ => 0);
        if (poll.AllowOther)
            results["Other"] = 0;
        foreach (var r in _responses.Where(r => r.PollId == pollId))
        {
            if (results.ContainsKey(r.SelectedOption))
                results[r.SelectedOption]++;
        }
        return results;
    }

    public List<string> GetOtherResponses(string pollId)
        => _responses
            .Where(r => r.PollId == pollId && r.SelectedOption == "Other" && !string.IsNullOrWhiteSpace(r.OtherText))
            .Select(r => r.OtherText!)
            .ToList();

    // --- Question Management ---
    public AudienceQuestion? SpotlightedQuestion { get; private set; }

    public void SpotlightQuestion(string? questionId)
    {
        if (questionId is not null && SpotlightedQuestion?.Id == questionId)
            questionId = null; // Toggle off if same question
        SpotlightedQuestion = questionId is null ? null : (_questions.TryGetValue(questionId, out var q) ? q : null);
        QuestionSpotlighted?.Invoke(SpotlightedQuestion);
    }

    public AudienceQuestion SubmitQuestion(string text, string? topicId = null, string? attendeeId = null)
    {
        var question = new AudienceQuestion
        {
            Text = text,
            TopicId = topicId,
            AttendeeId = attendeeId
        };
        _questions[question.Id] = question;
        QuestionReceived?.Invoke(question);
        return question;
    }

    public AudienceQuestion? AnswerQuestion(string questionId, string answer, bool isAiGenerated = false, string authorLabel = "Presenter")
    {
        if (!_questions.TryGetValue(questionId, out var question)) return null;
        question.Answers.Add(new QuestionAnswer
        {
            Text = answer,
            IsAiGenerated = isAiGenerated,
            AuthorLabel = authorLabel
        });
        QuestionAnswered?.Invoke(question);
        return question;
    }

    public void UpvoteQuestion(string questionId)
    {
        if (!_questions.TryGetValue(questionId, out var question)) return;
        question.Upvotes++;
        QuestionUpvoted?.Invoke(question);
    }

    public void RemoveQuestion(string questionId)
    {
        if (!_questions.TryRemove(questionId, out var question)) return;
        if (SpotlightedQuestion?.Id == questionId)
            SpotlightQuestion(null);
        QuestionRemoved?.Invoke(question);
    }

    public void MarkQuestionUnsafe(string questionId)
    {
        if (!_questions.TryGetValue(questionId, out var question)) return;
        question.IsSafe = false;
        QuestionModerated?.Invoke(question);
    }

    public void ApproveQuestion(string questionId)
    {
        if (!_questions.TryGetValue(questionId, out var question)) return;
        question.IsApprovedByPresenter = true;
        QuestionModerated?.Invoke(question);
    }

    public IReadOnlyList<AudienceQuestion> GetQuestionsForTopic(string topicId)
        => _questions.Values.Where(q => q.TopicId == topicId).ToList();

    public IReadOnlyList<AudienceQuestion> GetTopQuestions(int count = 10, bool includeUnsafe = false)
        => _questions.Values
            .Where(q => includeUnsafe || q.IsVisibleToAttendees)
            .OrderByDescending(q => q.Upvotes)
            .Take(count)
            .ToList();

    // --- Insight Management ---
    public Insight AddInsight(Insight insight)
    {
        _insights.Add(insight);
        InsightGenerated?.Invoke(insight);
        return insight;
    }

    public IReadOnlyList<Insight> GetInsightsForPoll(string pollId)
        => _insights.Where(i => i.PollId == pollId).ToList();

    public IReadOnlyList<Insight> GetInsightsForTopic(string topicId)
        => _insights.Where(i => i.TopicId == topicId).ToList();

    public IReadOnlyList<Insight> GetAllInsights()
        => _insights.ToList();

    public void ClearRuntimeData()
    {
        _polls.Clear();
        _responses.Clear();
        _questions.Clear();
        _insights.Clear();
    }

    /// <summary>
    /// Restores persisted runtime state without firing events.
    /// Used during startup to rehydrate sessions from PostgreSQL.
    /// </summary>
    public void RestoreState(
        IEnumerable<Poll>? polls = null,
        IEnumerable<PollResponse>? responses = null,
        IEnumerable<AudienceQuestion>? questions = null,
        IEnumerable<Insight>? insights = null,
        List<Slide>? slides = null)
    {
        if (polls is not null)
            foreach (var poll in polls)
                _polls[poll.Id] = poll;

        if (responses is not null)
            foreach (var response in responses)
                _responses.Add(response);

        if (questions is not null)
            foreach (var question in questions)
                _questions[question.Id] = question;

        if (insights is not null)
            foreach (var insight in insights)
                _insights.Add(insight);

        if (slides is not null)
            LoadSlides(slides);
    }

    private void SyncTopicToSlide()
    {
        var slide = ActiveSlide;
        if (slide is null || string.IsNullOrEmpty(slide.TopicId)) return;

        var currentActiveTopicId = Session.ActiveTopicId;
        if (slide.TopicId == currentActiveTopicId) return;

        // Find the topic for this slide
        var topic = Session.Topics.FirstOrDefault(t => t.Id == slide.TopicId);
        if (topic is null) return;

        lock (_lock)
        {
            // Mark previous active topic as completed (if any)
            var previousActive = Session.Topics.FirstOrDefault(t => t.Status == TopicStatus.Active);
            if (previousActive is not null && previousActive.Id != topic.Id)
                previousActive.Status = TopicStatus.Completed;

            // Activate the new topic
            topic.Status = TopicStatus.Active;
            Session.ActiveTopicId = topic.Id;
        }

        TopicActivated?.Invoke(topic.Id);
    }
}
