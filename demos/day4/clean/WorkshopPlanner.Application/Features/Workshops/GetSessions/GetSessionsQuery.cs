using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.GetSessions;

public sealed record GetSessionsQuery(int WorkshopId) : IRequest<Result<IReadOnlyList<WorkshopSessionResponse>>>;
