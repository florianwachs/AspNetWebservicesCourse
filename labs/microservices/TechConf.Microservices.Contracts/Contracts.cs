namespace TechConf.Microservices.Contracts;

public static class DemoIds
{
    public static readonly Guid CloudNativeSummit = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ApiCraftDay = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ArchitectureClinic = Guid.Parse("33333333-3333-3333-3333-333333333333");
}

public sealed record EventSummary(
    Guid Id,
    string Name,
    string City,
    DateOnly StartsOn,
    DateOnly EndsOn,
    string Status);

public sealed record SessionSummary(
    Guid Id,
    Guid EventId,
    string Title,
    string Speaker,
    DateTimeOffset StartsAt,
    int SeatsAvailable);

public sealed record RegistrationRequest(
    Guid EventId,
    Guid SessionId,
    string AttendeeName,
    string AttendeeEmail);

public sealed record RegistrationCreatedResponse(
    Guid RegistrationId,
    Guid EventId,
    Guid SessionId,
    string AttendeeName,
    string AttendeeEmail,
    DateTimeOffset RegisteredAt);

public sealed record RegistrationCreated(
    Guid RegistrationId,
    Guid EventId,
    Guid SessionId,
    string AttendeeName,
    string AttendeeEmail,
    DateTimeOffset RegisteredAt);

public sealed record NotificationSummary(
    Guid Id,
    Guid RegistrationId,
    string Recipient,
    string Subject,
    string Channel,
    DateTimeOffset CreatedAt,
    string Status);

public sealed record RecommendationSummary(
    string Id,
    Guid EventId,
    string Title,
    string Reason,
    int Confidence);

public sealed record DashboardResponse(
    int EventCount,
    int SessionCount,
    int RegistrationCount,
    int NotificationCount,
    int RecommendationCount,
    IReadOnlyList<ServiceSnapshot> Services);

public sealed record ServiceSnapshot(
    string Name,
    string Capability,
    string Runtime,
    string DataStore,
    string Communication,
    string Status);

public sealed record CountResponse(int Count);
