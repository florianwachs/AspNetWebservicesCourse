using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.AddSession;

public sealed class AddSessionHandler(WorkshopStore store)
    : IRequestHandler<AddSessionCommand, Result<SessionCreatedResponse>>
{
    public Task<Result<SessionCreatedResponse>> Handle(AddSessionCommand request, CancellationToken ct)
    {
        var workshop = store.Workshops.FirstOrDefault(item => item.Id == request.WorkshopId);
        if (workshop is null)
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(ErrorType.NotFound, $"Workshop {request.WorkshopId} was not found."));

        if (workshop.IsPublished)
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(ErrorType.Conflict, "Published workshops cannot be changed."));

        // TODO: Task 3 - Complete the command flow.
        // Reuse the CreateWorkshopHandler pattern:
        // 1. reject duplicate session titles inside the selected workshop,
        // 2. reject agendas above 480 total minutes,
        // 3. create the new SessionState and return SessionCreatedResponse.
        return Task.FromResult(
            Result<SessionCreatedResponse>.Failure(
                ErrorType.Validation,
                "TODO: implement AddSessionHandler."));
    }
}
