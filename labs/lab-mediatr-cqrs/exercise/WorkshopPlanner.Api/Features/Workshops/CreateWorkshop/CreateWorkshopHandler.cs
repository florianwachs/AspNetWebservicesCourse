using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;

public sealed class CreateWorkshopHandler(WorkshopStore store)
    : IRequestHandler<CreateWorkshopCommand, Result<WorkshopCreatedResponse>>
{
    public Task<Result<WorkshopCreatedResponse>> Handle(CreateWorkshopCommand request, CancellationToken ct)
    {
        if (store.Workshops.Any(item => item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult(
                Result<WorkshopCreatedResponse>.Failure(
                    ErrorType.Conflict,
                    $"A workshop named '{request.Title}' already exists."));

        var workshop = new WorkshopState(
            store.GetNextWorkshopId(),
            request.Title.Trim(),
            request.City.Trim(),
            request.MaxAttendees);

        store.Workshops.Add(workshop);

        return Task.FromResult(Result<WorkshopCreatedResponse>.Success(new WorkshopCreatedResponse(workshop.Id)));
    }
}
