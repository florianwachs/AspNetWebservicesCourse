using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals;

public sealed record ProposalListItemResponse(
    int Id,
    string Title,
    string Track,
    int DurationMinutes,
    ProposalStatus Status,
    string SpeakerName,
    int SpeakerProfileId,
    string EventName,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record ProposalDetailResponse(
    int Id,
    string Title,
    string Abstract,
    string Track,
    int DurationMinutes,
    ProposalStatus Status,
    int SpeakerProfileId,
    string SpeakerName,
    int EventId,
    string EventName,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? ReviewedAtUtc,
    string? DecisionNote);

public sealed record ReviewProposalRequest(string? Note);
