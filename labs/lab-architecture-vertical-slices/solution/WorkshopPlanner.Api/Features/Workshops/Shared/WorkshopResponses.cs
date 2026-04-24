namespace WorkshopPlanner.Api.Features.Workshops.Shared;

public sealed record WorkshopSummaryResponse(
    int Id,
    string Title,
    string City,
    int MaxAttendees,
    int SessionCount,
    bool IsPublished);

public sealed record WorkshopSessionResponse(int Id, string Title, string SpeakerName, int DurationMinutes);

public sealed record WorkshopDetailResponse(
    int Id,
    string Title,
    string City,
    int MaxAttendees,
    bool IsPublished,
    IReadOnlyCollection<WorkshopSessionResponse> Sessions);

public sealed record WorkshopCreatedResponse(int Id);

public sealed record SessionCreatedResponse(int WorkshopId, int SessionId);

public sealed record PublishWorkshopResponse(int Id, string Title, string Status, DateTime PublishedOnUtc);

internal static class WorkshopMappings
{
    public static WorkshopSummaryResponse ToSummary(this WorkshopState workshop) =>
        new(
            workshop.Id,
            workshop.Title,
            workshop.City,
            workshop.MaxAttendees,
            workshop.Sessions.Count,
            workshop.IsPublished);

    public static WorkshopDetailResponse ToDetail(this WorkshopState workshop) =>
        new(
            workshop.Id,
            workshop.Title,
            workshop.City,
            workshop.MaxAttendees,
            workshop.IsPublished,
            workshop.Sessions
                .Select(session => new WorkshopSessionResponse(
                    session.Id,
                    session.Title,
                    session.SpeakerName,
                    session.DurationMinutes))
                .ToArray());
}
