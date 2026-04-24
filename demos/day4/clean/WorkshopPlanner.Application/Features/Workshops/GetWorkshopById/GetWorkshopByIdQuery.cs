using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;

namespace WorkshopPlanner.Application.Features.Workshops.GetWorkshopById;

public sealed record GetWorkshopByIdQuery(int WorkshopId) : IRequest<WorkshopPlanner.Domain.Common.Result<WorkshopDetailResponse>>;
