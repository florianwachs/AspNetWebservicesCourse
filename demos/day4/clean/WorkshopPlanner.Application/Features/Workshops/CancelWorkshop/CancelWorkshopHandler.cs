using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CancelWorkshop;

public sealed class CancelWorkshopHandler(IWorkshopRepository repository)
    : IRequestHandler<CancelWorkshopCommand, Result<WorkshopCanceledResponse>>
{
    public async Task<Result<WorkshopCanceledResponse>> Handle(CancelWorkshopCommand command, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<WorkshopCanceledResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var cancelResult = workshop.Cancel(DateTime.UtcNow);
        if (!cancelResult.IsSuccess)
            return Result<WorkshopCanceledResponse>.Failure(cancelResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<WorkshopCanceledResponse>.Success(
            new WorkshopCanceledResponse(workshop.Id, workshop.Title, "Canceled", workshop.CanceledOnUtc!.Value));
    }
}
