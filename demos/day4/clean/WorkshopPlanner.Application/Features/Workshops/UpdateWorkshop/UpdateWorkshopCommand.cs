using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateWorkshop;

public sealed record UpdateWorkshopCommand(int WorkshopId, string Title, string City, int MaxAttendees)
    : IRequest<Result<WorkshopUpdatedResponse>>;
