using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CancelWorkshop;

public sealed record CancelWorkshopCommand(int WorkshopId) : IRequest<Result<WorkshopCanceledResponse>>;
