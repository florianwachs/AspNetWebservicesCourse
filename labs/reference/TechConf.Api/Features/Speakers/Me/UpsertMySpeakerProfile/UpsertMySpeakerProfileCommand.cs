using MediatR;

namespace TechConf.Api.Features.Speakers.Me.UpsertMySpeakerProfile;

public sealed record UpsertMySpeakerProfileCommand(
    string DisplayName,
    string Tagline,
    string Bio,
    string Company,
    string City,
    string? WebsiteUrl,
    string? PhotoUrl) : IRequest<MySpeakerProfileResponse>;
