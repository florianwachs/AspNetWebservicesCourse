using MediatR;

namespace TechConf.Api.Features.Speakers.Me.GetMySpeakerProfile;

public sealed record GetMySpeakerProfileQuery() : IRequest<MySpeakerProfileResponse>;
