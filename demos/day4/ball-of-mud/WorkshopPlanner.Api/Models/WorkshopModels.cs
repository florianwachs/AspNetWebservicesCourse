namespace WorkshopPlanner.Api.Models;

public sealed class Workshop(int id, string title, string city, int maxAttendees)
{
    public int Id { get; } = id;
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public int MaxAttendees { get; set; } = maxAttendees;
    public DateTime? PublishedOnUtc { get; set; }
    public DateTime? CanceledOnUtc { get; set; }
    public bool IsPublished => PublishedOnUtc is not null;
    public bool IsCanceled => CanceledOnUtc is not null;
    public List<WorkshopSession> Sessions { get; } = [];
    public List<WorkshopRegistration> Registrations { get; } = [];
}

public sealed class WorkshopSession(int id, string title, string speakerName, int durationMinutes)
{
    public int Id { get; } = id;
    public string Title { get; set; } = title;
    public string SpeakerName { get; set; } = speakerName;
    public int DurationMinutes { get; set; } = durationMinutes;
}

public enum RegistrationStatus
{
    Confirmed,
    Waitlisted,
    Cancelled,
    CheckedIn
}

public sealed class WorkshopRegistration(int id, string attendeeName, string attendeeEmail, RegistrationStatus status)
{
    public int Id { get; } = id;
    public string AttendeeName { get; set; } = attendeeName;
    public string AttendeeEmail { get; set; } = attendeeEmail;
    public RegistrationStatus Status { get; set; } = status;
}

public sealed record CreateWorkshopRequest(string Title, string City, int MaxAttendees);

public sealed record UpdateWorkshopRequest(string Title, string City, int MaxAttendees);

public sealed record AddSessionRequest(string Title, string SpeakerName, int DurationMinutes);

public sealed record UpdateSessionRequest(string Title, string SpeakerName, int DurationMinutes);

public sealed record CreateRegistrationRequest(string AttendeeName, string AttendeeEmail);

public sealed record WorkshopSummaryResponse(
    int Id,
    string Title,
    string City,
    int MaxAttendees,
    int SessionCount,
    int ActiveRegistrationCount,
    int WaitlistedRegistrationCount,
    bool IsCanceled,
    bool IsPublished);
