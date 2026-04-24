using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;

namespace WorkshopPlanner.Application.Features.Workshops.GetWorkshops;

public sealed record GetWorkshopsQuery() : IRequest<IReadOnlyList<WorkshopSummaryResponse>>;
