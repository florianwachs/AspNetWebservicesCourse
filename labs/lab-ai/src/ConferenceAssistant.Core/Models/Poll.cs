namespace ConferenceAssistant.Core.Models;

public class Poll
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? TopicId { get; set; }
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = [];
    public bool AllowOther { get; set; }
    public PollStatus Status { get; set; } = PollStatus.Draft;
    public PollSource Source { get; set; } = PollSource.Generated;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }
}

public enum PollStatus { Draft, Active, Closed }
public enum PollSource { Generated, Suggested, Custom }
