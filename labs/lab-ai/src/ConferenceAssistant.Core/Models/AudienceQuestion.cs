namespace ConferenceAssistant.Core.Models;

public class AudienceQuestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = "";
    public string? TopicId { get; set; }
    public List<QuestionAnswer> Answers { get; set; } = [];
    public string? AttendeeId { get; set; }
    public int Upvotes { get; set; }
    public DateTimeOffset AskedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsSafe { get; set; } = true;
    public bool IsApprovedByPresenter { get; set; }

    /// <summary>Whether this question should be visible to attendees.</summary>
    public bool IsVisibleToAttendees => IsSafe || IsApprovedByPresenter;
}
