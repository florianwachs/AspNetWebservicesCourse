namespace TechConf.Api.Models;

public record Event(int Id, string Name, DateTime Date, string City, string? Description = null);

public record CreateEventRequest(string Name, DateTime Date, string City, string? Description);

public record UpdateEventRequest(string Name, DateTime Date, string City, string? Description);
