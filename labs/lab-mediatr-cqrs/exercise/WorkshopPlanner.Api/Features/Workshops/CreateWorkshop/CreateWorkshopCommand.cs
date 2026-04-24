using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;

public sealed record CreateWorkshopCommand(string Title, string City, int MaxAttendees)
    : IRequest<Result<WorkshopCreatedResponse>>;
