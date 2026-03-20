namespace TechConf.Api.Models;

public record CreateEventRequest(string Name, DateTime Date, string City, string? Description);
public record UpdateEventRequest(string Name, DateTime Date, string City, string? Description);
public record EventDto(int Id, string Name, DateTime Date, string City, string? Description, int SessionCount);
public record EventDetailDto(int Id, string Name, DateTime Date, string City, string? Description, List<SessionDto> Sessions);
public record SessionDto(int Id, string Title, TimeSpan Duration, List<string> SpeakerNames);
