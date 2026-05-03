namespace TechConf.McpServer.Models;

public class Registration
{
    public Guid Id { get; set; }
    public DateTime RegisteredAt { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Guid AttendeeId { get; set; }
    public Attendee Attendee { get; set; } = null!;
}
