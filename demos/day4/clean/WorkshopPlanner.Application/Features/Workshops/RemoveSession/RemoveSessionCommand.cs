using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.RemoveSession;

public sealed record RemoveSessionCommand(int WorkshopId, int SessionId) : IRequest<Result<SessionRemovedResponse>>;
