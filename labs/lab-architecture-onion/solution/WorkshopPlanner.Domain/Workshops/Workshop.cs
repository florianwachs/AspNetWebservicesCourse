using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Domain.Workshops;

public sealed class Workshop
{
    private readonly List<WorkshopSession> _sessions = [];

    private Workshop(int id, string title, string city, int maxAttendees)
    {
        Id = id;
        Title = title;
        City = city;
        MaxAttendees = maxAttendees;
    }

    public int Id { get; }

    public string Title { get; }

    public string City { get; }

    public int MaxAttendees { get; }

    public DateTime? PublishedOnUtc { get; private set; }

    public bool IsPublished => PublishedOnUtc is not null;

    public IReadOnlyCollection<WorkshopSession> Sessions => _sessions.AsReadOnly();

    public static Result<Workshop> Create(int id, string title, string city, int maxAttendees)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Workshop>.Failure(ErrorType.Validation, "Title is required.");

        if (string.IsNullOrWhiteSpace(city))
            return Result<Workshop>.Failure(ErrorType.Validation, "City is required.");

        if (maxAttendees < 5)
            return Result<Workshop>.Failure(ErrorType.Validation, "Max attendees must be at least 5.");

        return Result<Workshop>.Success(new Workshop(id, title.Trim(), city.Trim(), maxAttendees));
    }

    public Result AddSession(int sessionId, string title, string speakerName, int durationMinutes)
    {
        if (IsPublished)
            return Result.Failure(ErrorType.Conflict, "Published workshops cannot be changed.");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(ErrorType.Validation, "Session title is required.");

        if (string.IsNullOrWhiteSpace(speakerName))
            return Result.Failure(ErrorType.Validation, "Speaker name is required.");

        if (durationMinutes is < 30 or > 180)
            return Result.Failure(ErrorType.Validation, "Session duration must be between 30 and 180 minutes.");

        if (_sessions.Any(session => session.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure(ErrorType.Conflict, $"A session named '{title}' already exists in this workshop.");

        if (_sessions.Sum(session => session.DurationMinutes) + durationMinutes > 480)
            return Result.Failure(ErrorType.Validation, "A workshop cannot exceed 480 total session minutes.");

        _sessions.Add(new WorkshopSession(sessionId, title.Trim(), speakerName.Trim(), durationMinutes));
        return Result.Success();
    }

    public Result Publish(DateTime publishedOnUtc)
    {
        if (IsPublished)
            return Result.Failure(ErrorType.Conflict, "The workshop is already published.");

        if (_sessions.Count == 0)
            return Result.Failure(ErrorType.Validation, "Add at least one session before publishing.");

        if (_sessions.Sum(session => session.DurationMinutes) < 60)
            return Result.Failure(ErrorType.Validation, "A published workshop needs at least 60 total minutes of sessions.");

        PublishedOnUtc = publishedOnUtc;
        return Result.Success();
    }
}
