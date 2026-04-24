namespace WorkshopPlanner.Api.Models;

public sealed class Workshop(int id, string title, string city, int maxAttendees)
{
    public int Id { get; } = id;
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public int MaxAttendees { get; set; } = maxAttendees;
    public DateTime? PublishedOnUtc { get; set; }
    public List<WorkshopSession> Sessions { get; } = [];
}

public sealed record WorkshopSession(int Id, string Title, string SpeakerName, int DurationMinutes);

public sealed record CreateWorkshopRequest(string Title, string City, int MaxAttendees);

public sealed record AddSessionRequest(string Title, string SpeakerName, int DurationMinutes);

public sealed record WorkshopSummaryResponse(
    int Id,
    string Title,
    string City,
    int MaxAttendees,
    int SessionCount,
    bool IsPublished);
