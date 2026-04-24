using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.AddSession;

public sealed record AddSessionCommand(int WorkshopId, string Title, string SpeakerName, int DurationMinutes)
    : IRequest<Result<SessionCreatedResponse>>;
