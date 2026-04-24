using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.PublishWorkshop;

public sealed record PublishWorkshopCommand(int WorkshopId) : IRequest<Result<PublishWorkshopResponse>>;
