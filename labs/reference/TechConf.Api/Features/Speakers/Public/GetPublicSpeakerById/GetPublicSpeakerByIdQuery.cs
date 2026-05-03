using MediatR;

namespace TechConf.Api.Features.Speakers.Public.GetPublicSpeakerById;

public sealed record GetPublicSpeakerByIdQuery(int SpeakerId) : IRequest<SpeakerDetailV2>;
