using WorkshopPlanner.Domain.Workshops;

namespace WorkshopPlanner.Application.Features.Workshops.Shared;

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

public sealed record WorkshopSessionResponse(int Id, string Title, string SpeakerName, int DurationMinutes);

public sealed record WorkshopRegistrationResponse(int Id, string AttendeeName, string AttendeeEmail, string Status);

public sealed record WorkshopDetailResponse(
    int Id,
    string Title,
    string City,
    int MaxAttendees,
    DateTime? PublishedOnUtc,
    DateTime? CanceledOnUtc,
    bool IsPublished,
    bool IsCanceled,
    IReadOnlyCollection<WorkshopSessionResponse> Sessions,
    IReadOnlyCollection<WorkshopRegistrationResponse> Registrations);

public sealed record WorkshopCreatedResponse(int Id);

public sealed record WorkshopUpdatedResponse(int Id, string Title, string City, int MaxAttendees);

public sealed record WorkshopCanceledResponse(int Id, string Title, string Status, DateTime CanceledOnUtc);

public sealed record SessionCreatedResponse(int WorkshopId, int SessionId);

public sealed record SessionUpdatedResponse(int WorkshopId, int SessionId);

public sealed record SessionRemovedResponse(int WorkshopId, int SessionId);

public sealed record PublishWorkshopResponse(int Id, string Title, string Status, DateTime PublishedOnUtc);

public sealed record RegistrationCreatedResponse(int WorkshopId, int RegistrationId, string Status);

public sealed record RegistrationCancelledResponse(int WorkshopId, int RegistrationId, string Status, int? PromotedRegistrationId);

public sealed record RegistrationCheckedInResponse(int WorkshopId, int RegistrationId, string Status);

internal static class WorkshopMappings
{
    public static WorkshopSummaryResponse ToSummary(this Workshop workshop) =>
        new(
            workshop.Id,
            workshop.Title,
            workshop.City,
            workshop.MaxAttendees,
            workshop.Sessions.Count,
            workshop.Registrations.Count(registration => registration.IsSeatHolding),
            workshop.Registrations.Count(registration => registration.Status == RegistrationStatus.Waitlisted),
            workshop.IsCanceled,
            workshop.IsPublished);

    public static WorkshopDetailResponse ToDetail(this Workshop workshop) =>
        new(
            workshop.Id,
            workshop.Title,
            workshop.City,
            workshop.MaxAttendees,
            workshop.PublishedOnUtc,
            workshop.CanceledOnUtc,
            workshop.IsPublished,
            workshop.IsCanceled,
            workshop.Sessions
                .Select(session => new WorkshopSessionResponse(
                    session.Id,
                    session.Title,
                    session.SpeakerName,
                    session.DurationMinutes))
                .ToArray(),
            workshop.Registrations
                .Select(registration => new WorkshopRegistrationResponse(
                    registration.Id,
                    registration.AttendeeName,
                    registration.AttendeeEmail,
                    registration.Status.ToString()))
                .ToArray());
}
