namespace TechConf.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
}

public record EventDto(int Id, string Name, DateTime Date, string City, string? Description);

public record CreateEventRequest(string Name, DateTime Date, string City, string? Description);

public record UpdateEventRequest(string Name, DateTime Date, string City, string? Description);
