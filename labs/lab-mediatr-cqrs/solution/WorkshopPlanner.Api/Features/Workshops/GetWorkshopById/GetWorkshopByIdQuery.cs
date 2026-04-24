using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshopById;

public sealed record GetWorkshopByIdQuery(int WorkshopId) : IRequest<Result<WorkshopDetailResponse>>;
