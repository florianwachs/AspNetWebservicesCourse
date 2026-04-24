namespace WorkshopPlanner.Api.Features.Workshops.Shared;

public sealed class WorkshopState(int id, string title, string city, int maxAttendees)
{
    public int Id { get; } = id;
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public int MaxAttendees { get; set; } = maxAttendees;
    public DateTime? PublishedOnUtc { get; set; }
    public bool IsPublished => PublishedOnUtc is not null;
    public List<SessionState> Sessions { get; } = [];
}

public sealed record SessionState(int Id, string Title, string SpeakerName, int DurationMinutes);
