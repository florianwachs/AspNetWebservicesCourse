namespace ConferenceAssistant.Core.Models;

public class ConferenceSession
{
    public string Id { get; set; } = "";
    public string SessionCode { get; set; } = "";
    public string HostPin { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public SessionStatus Status { get; set; } = SessionStatus.Setup;
    public List<SessionTopic> Topics { get; set; } = [];
    public string? ActiveTopicId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
}

public enum SessionStatus { Setup, Live, Completed }
