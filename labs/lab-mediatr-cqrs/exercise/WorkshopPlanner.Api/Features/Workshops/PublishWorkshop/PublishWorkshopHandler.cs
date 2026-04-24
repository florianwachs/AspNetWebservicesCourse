using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.PublishWorkshop;

public sealed class PublishWorkshopHandler(WorkshopStore store)
    : IRequestHandler<PublishWorkshopCommand, Result<PublishWorkshopResponse>>
{
    public Task<Result<PublishWorkshopResponse>> Handle(PublishWorkshopCommand request, CancellationToken ct)
    {
        var workshop = store.Workshops.FirstOrDefault(item => item.Id == request.WorkshopId);
        if (workshop is null)
            return Task.FromResult(
                Result<PublishWorkshopResponse>.Failure(ErrorType.NotFound, $"Workshop {request.WorkshopId} was not found."));

        if (workshop.IsPublished)
            return Task.FromResult(
                Result<PublishWorkshopResponse>.Failure(ErrorType.Conflict, "The workshop is already published."));

        // TODO: Task 4 - Complete the publish rules.
        // A workshop must have at least one session and at least 60 total minutes before it can be published.
        // Then set PublishedOnUtc and return the published response DTO.
        return Task.FromResult(
            Result<PublishWorkshopResponse>.Failure(
                ErrorType.Validation,
                "TODO: implement PublishWorkshopHandler."));
    }
}
