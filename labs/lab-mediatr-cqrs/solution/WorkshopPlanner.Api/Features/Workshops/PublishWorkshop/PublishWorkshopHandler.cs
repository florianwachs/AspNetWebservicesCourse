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

        if (workshop.Sessions.Count == 0)
            return Task.FromResult(
                Result<PublishWorkshopResponse>.Failure(ErrorType.Validation, "Add at least one session before publishing."));

        if (workshop.Sessions.Sum(session => session.DurationMinutes) < 60)
            return Task.FromResult(
                Result<PublishWorkshopResponse>.Failure(
                    ErrorType.Validation,
                    "A published workshop needs at least 60 total minutes of sessions."));

        workshop.PublishedOnUtc = DateTime.UtcNow;

        return Task.FromResult(
            Result<PublishWorkshopResponse>.Success(
                new PublishWorkshopResponse(workshop.Id, workshop.Title, "Published", workshop.PublishedOnUtc.Value)));
    }
}
