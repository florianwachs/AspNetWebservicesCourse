using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CancelRegistration;

public sealed class CancelRegistrationHandler(IWorkshopRepository repository)
    : IRequestHandler<CancelRegistrationCommand, Result<RegistrationCancelledResponse>>
{
    public async Task<Result<RegistrationCancelledResponse>> Handle(CancelRegistrationCommand command, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<RegistrationCancelledResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var cancelResult = workshop.CancelRegistration(command.RegistrationId);
        if (!cancelResult.IsSuccess)
            return Result<RegistrationCancelledResponse>.Failure(cancelResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<RegistrationCancelledResponse>.Success(
            new RegistrationCancelledResponse(command.WorkshopId, command.RegistrationId, "Cancelled", cancelResult.Value));
    }
}
