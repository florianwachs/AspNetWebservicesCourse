using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Domain.Workshops;

public sealed class Workshop
{
    private readonly List<WorkshopSession> _sessions = [];
    private readonly List<WorkshopRegistration> _registrations = [];

    private Workshop(int id, string title, string city, int maxAttendees)
    {
        Id = id;
        Title = title;
        City = city;
        MaxAttendees = maxAttendees;
    }

    public int Id { get; }

    public string Title { get; private set; }

    public string City { get; private set; }

    public int MaxAttendees { get; private set; }

    public DateTime? PublishedOnUtc { get; private set; }

    public DateTime? CanceledOnUtc { get; private set; }

    public bool IsPublished => PublishedOnUtc is not null;

    public bool IsCanceled => CanceledOnUtc is not null;

    public IReadOnlyCollection<WorkshopSession> Sessions => _sessions.AsReadOnly();

    public IReadOnlyCollection<WorkshopRegistration> Registrations => _registrations.AsReadOnly();

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

    public Result UpdateDetails(string title, string city, int maxAttendees)
    {
        var stateGuard = GuardEditable();
        if (!stateGuard.IsSuccess)
            return stateGuard;

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(ErrorType.Validation, "Title is required.");

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure(ErrorType.Validation, "City is required.");

        if (maxAttendees < 5)
            return Result.Failure(ErrorType.Validation, "Max attendees must be at least 5.");

        var activeRegistrations = _registrations.Count(registration => registration.IsSeatHolding);
        if (maxAttendees < activeRegistrations)
            return Result.Failure(ErrorType.Conflict, "Max attendees cannot be reduced below the number of active registrations.");

        Title = title.Trim();
        City = city.Trim();
        MaxAttendees = maxAttendees;
        return Result.Success();
    }

    public Result AddSession(int sessionId, string title, string speakerName, int durationMinutes)
    {
        var stateGuard = GuardEditable();
        if (!stateGuard.IsSuccess)
            return stateGuard;

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

    public Result UpdateSession(int sessionId, string title, string speakerName, int durationMinutes)
    {
        var stateGuard = GuardEditable();
        if (!stateGuard.IsSuccess)
            return stateGuard;

        var session = _sessions.FirstOrDefault(item => item.Id == sessionId);
        if (session is null)
            return Result.Failure(ErrorType.NotFound, $"Session {sessionId} was not found.");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(ErrorType.Validation, "Session title is required.");

        if (string.IsNullOrWhiteSpace(speakerName))
            return Result.Failure(ErrorType.Validation, "Speaker name is required.");

        if (durationMinutes is < 30 or > 180)
            return Result.Failure(ErrorType.Validation, "Session duration must be between 30 and 180 minutes.");

        if (_sessions.Any(item => item.Id != sessionId && item.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure(ErrorType.Conflict, $"A session named '{title}' already exists in this workshop.");

        var otherDuration = _sessions.Where(item => item.Id != sessionId).Sum(item => item.DurationMinutes);
        if (otherDuration + durationMinutes > 480)
            return Result.Failure(ErrorType.Validation, "A workshop cannot exceed 480 total session minutes.");

        session.Update(title.Trim(), speakerName.Trim(), durationMinutes);
        return Result.Success();
    }

    public Result RemoveSession(int sessionId)
    {
        var stateGuard = GuardEditable();
        if (!stateGuard.IsSuccess)
            return stateGuard;

        var session = _sessions.FirstOrDefault(item => item.Id == sessionId);
        if (session is null)
            return Result.Failure(ErrorType.NotFound, $"Session {sessionId} was not found.");

        _sessions.Remove(session);
        return Result.Success();
    }

    public Result Publish(DateTime publishedOnUtc)
    {
        if (IsCanceled)
            return Result.Failure(ErrorType.Conflict, "Canceled workshops cannot be published.");

        if (IsPublished)
            return Result.Failure(ErrorType.Conflict, "The workshop is already published.");

        if (_sessions.Count == 0)
            return Result.Failure(ErrorType.Validation, "Add at least one session before publishing.");

        if (_sessions.Sum(session => session.DurationMinutes) < 60)
            return Result.Failure(ErrorType.Validation, "A published workshop needs at least 60 total minutes of sessions.");

        PublishedOnUtc = publishedOnUtc;
        return Result.Success();
    }

    public Result Cancel(DateTime canceledOnUtc)
    {
        if (IsCanceled)
            return Result.Failure(ErrorType.Conflict, "The workshop is already canceled.");

        CanceledOnUtc = canceledOnUtc;
        return Result.Success();
    }

    public Result<WorkshopRegistration> AddRegistration(int registrationId, string attendeeName, string attendeeEmail)
    {
        if (IsCanceled)
            return Result<WorkshopRegistration>.Failure(ErrorType.Conflict, "Canceled workshops do not accept registrations.");

        if (!IsPublished)
            return Result<WorkshopRegistration>.Failure(ErrorType.Conflict, "Only published workshops accept registrations.");

        if (string.IsNullOrWhiteSpace(attendeeName))
            return Result<WorkshopRegistration>.Failure(ErrorType.Validation, "Attendee name is required.");

        if (string.IsNullOrWhiteSpace(attendeeEmail))
            return Result<WorkshopRegistration>.Failure(ErrorType.Validation, "Attendee email is required.");

        var normalizedEmail = attendeeEmail.Trim();
        if (!normalizedEmail.Contains('@'))
            return Result<WorkshopRegistration>.Failure(ErrorType.Validation, "Attendee email must be a valid email address.");

        if (_registrations.Any(registration =>
                registration.Status != RegistrationStatus.Cancelled &&
                registration.AttendeeEmail.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<WorkshopRegistration>.Failure(
                ErrorType.Conflict,
                $"A registration for '{normalizedEmail}' already exists in this workshop.");
        }

        var status = _registrations.Count(registration => registration.IsSeatHolding) >= MaxAttendees
            ? RegistrationStatus.Waitlisted
            : RegistrationStatus.Confirmed;

        var registration = new WorkshopRegistration(
            registrationId,
            attendeeName.Trim(),
            normalizedEmail,
            status);

        _registrations.Add(registration);
        return Result<WorkshopRegistration>.Success(registration);
    }

    public Result<int?> CancelRegistration(int registrationId)
    {
        var registration = _registrations.FirstOrDefault(item => item.Id == registrationId);
        if (registration is null)
            return Result<int?>.Failure(ErrorType.NotFound, $"Registration {registrationId} was not found.");

        if (registration.Status == RegistrationStatus.Cancelled)
            return Result<int?>.Failure(ErrorType.Conflict, "The registration is already canceled.");

        if (registration.Status == RegistrationStatus.CheckedIn)
            return Result<int?>.Failure(ErrorType.Conflict, "Checked-in registrations cannot be canceled.");

        var wasSeatHolding = registration.IsSeatHolding;
        registration.Cancel();

        if (!wasSeatHolding)
            return Result<int?>.Success(null);

        var waitlisted = _registrations.FirstOrDefault(item => item.Status == RegistrationStatus.Waitlisted);
        if (waitlisted is null)
            return Result<int?>.Success(null);

        waitlisted.PromoteFromWaitlist();
        return Result<int?>.Success(waitlisted.Id);
    }

    public Result CheckInRegistration(int registrationId)
    {
        if (IsCanceled)
            return Result.Failure(ErrorType.Conflict, "Canceled workshops do not allow check-in.");

        if (!IsPublished)
            return Result.Failure(ErrorType.Conflict, "Only published workshops allow check-in.");

        var registration = _registrations.FirstOrDefault(item => item.Id == registrationId);
        if (registration is null)
            return Result.Failure(ErrorType.NotFound, $"Registration {registrationId} was not found.");

        return registration.Status switch
        {
            RegistrationStatus.Waitlisted => Result.Failure(ErrorType.Conflict, "Waitlisted registrations cannot be checked in."),
            RegistrationStatus.Cancelled => Result.Failure(ErrorType.Conflict, "Canceled registrations cannot be checked in."),
            RegistrationStatus.CheckedIn => Result.Failure(ErrorType.Conflict, "The registration is already checked in."),
            _ => CheckInConfirmedRegistration(registration)
        };
    }

    private Result CheckInConfirmedRegistration(WorkshopRegistration registration)
    {
        registration.CheckIn();
        return Result.Success();
    }

    private Result GuardEditable()
    {
        if (IsCanceled)
            return Result.Failure(ErrorType.Conflict, "Canceled workshops cannot be changed.");

        if (IsPublished)
            return Result.Failure(ErrorType.Conflict, "Published workshops cannot be changed.");

        return Result.Success();
    }
}
