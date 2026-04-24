using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshops;

public sealed record GetWorkshopsQuery() : IRequest<Result<IReadOnlyList<WorkshopSummaryResponse>>>;
