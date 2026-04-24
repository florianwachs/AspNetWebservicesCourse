using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.PublishWorkshop;

public sealed record PublishWorkshopCommand(int WorkshopId) : IRequest<Result<PublishWorkshopResponse>>;
