namespace WorkshopPlanner.Domain.Workshops;

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

    public string AttendeeName { get; } = attendeeName;

    public string AttendeeEmail { get; } = attendeeEmail;

    public RegistrationStatus Status { get; private set; } = status;

    public bool IsSeatHolding => Status is RegistrationStatus.Confirmed or RegistrationStatus.CheckedIn;

    public void Cancel() => Status = RegistrationStatus.Cancelled;

    public void PromoteFromWaitlist() => Status = RegistrationStatus.Confirmed;

    public void CheckIn() => Status = RegistrationStatus.CheckedIn;
}
