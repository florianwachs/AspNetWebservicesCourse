using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.AddSession;

public sealed record AddSessionCommand(int WorkshopId, string Title, string SpeakerName, int DurationMinutes)
    : IRequest<Result<SessionCreatedResponse>>;
