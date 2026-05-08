namespace ConferenceAssistant.Core.Models;

public class PollResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PollId { get; set; } = "";
    public string SelectedOption { get; set; } = "";
    public string? OtherText { get; set; }
    public string? AttendeeId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
