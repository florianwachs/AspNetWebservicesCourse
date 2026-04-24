using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateSession;

public sealed record UpdateSessionCommand(int WorkshopId, int SessionId, string Title, string SpeakerName, int DurationMinutes)
    : IRequest<Result<SessionUpdatedResponse>>;
