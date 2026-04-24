using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CreateWorkshop;

public sealed record CreateWorkshopCommand(string Title, string City, int MaxAttendees)
    : IRequest<Result<WorkshopCreatedResponse>>;
