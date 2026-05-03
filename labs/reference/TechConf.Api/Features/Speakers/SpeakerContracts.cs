namespace TechConf.Api.Features.Speakers;

public sealed record SpeakerSummaryV1(int Id, string Name);

public sealed record SpeakerSummaryV2(
    int Id,
    string Name,
    string Tagline,
    string Company,
    string City,
    int AcceptedTalkCount,
    IReadOnlyList<string> RecentTalks);

public sealed record PublicSpeakerSummary(
    int Id,
    string Name,
    string Tagline,
    string Company,
    string City,
    int AcceptedTalkCount,
    IReadOnlyList<string> RecentTalks,
    DateTimeOffset UpdatedAtUtc);

public sealed record SpeakerDetailV2(
    int Id,
    string Name,
    string Tagline,
    string Bio,
    string Company,
    string City,
    string? WebsiteUrl,
    string? PhotoUrl,
    int AcceptedTalkCount,
    IReadOnlyList<string> RecentTalks,
    DateTimeOffset UpdatedAtUtc);

public sealed record MySpeakerProfileResponse(
    int Id,
    string DisplayName,
    string Tagline,
    string Bio,
    string Company,
    string City,
    string Email,
    string? WebsiteUrl,
    string? PhotoUrl,
    int ProposalCount,
    DateTimeOffset UpdatedAtUtc);
