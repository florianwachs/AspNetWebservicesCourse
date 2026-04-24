using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.PublishWorkshop;

public sealed class PublishWorkshopHandler(IWorkshopRepository repository)
    : IRequestHandler<PublishWorkshopCommand, Result<PublishWorkshopResponse>>
{
    public async Task<Result<PublishWorkshopResponse>> Handle(PublishWorkshopCommand command, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<PublishWorkshopResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var publishResult = workshop.Publish(DateTime.UtcNow);
        if (!publishResult.IsSuccess)
            return Result<PublishWorkshopResponse>.Failure(publishResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<PublishWorkshopResponse>.Success(
            new PublishWorkshopResponse(workshop.Id, workshop.Title, "Published", workshop.PublishedOnUtc!.Value));
    }
}
