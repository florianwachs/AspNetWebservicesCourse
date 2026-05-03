namespace TechConf.Api.Models;

public record Event(
    int Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    Venue Venue,
    int RegisteredCount,
    EventStatus Status);

public record Venue(string Name, string Address, string City, int Capacity);

public enum EventStatus
{
    Draft,
    Published,
    Cancelled
}
