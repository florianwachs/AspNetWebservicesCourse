using System.ComponentModel.DataAnnotations;

namespace TechConf.Api.Models;

public record EventListItemDto(int Id, string Name, DateTime Date, string City, string? Description, int SessionCount);

public record EventDetailDto(int Id, string Name, DateTime Date, string City, string? Description, List<SessionDto> Sessions);

public record SessionDto(int Id, string Title, int DurationMinutes, List<string> SpeakerNames);

public record CreateEventRequest(
    [property: Required, StringLength(200, MinimumLength = 3)] string Name,
    DateTime Date,
    [property: Required, StringLength(100, MinimumLength = 2)] string City,
    [property: StringLength(2000)] string? Description);

public record UpdateEventRequest(
    [property: Required, StringLength(200, MinimumLength = 3)] string Name,
    DateTime Date,
    [property: Required, StringLength(100, MinimumLength = 2)] string City,
    [property: StringLength(2000)] string? Description);
