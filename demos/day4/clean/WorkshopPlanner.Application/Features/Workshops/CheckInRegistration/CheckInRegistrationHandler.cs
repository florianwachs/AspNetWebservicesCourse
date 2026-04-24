using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CheckInRegistration;

public sealed class CheckInRegistrationHandler(IWorkshopRepository repository)
    : IRequestHandler<CheckInRegistrationCommand, Result<RegistrationCheckedInResponse>>
{
    public async Task<Result<RegistrationCheckedInResponse>> Handle(CheckInRegistrationCommand command, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<RegistrationCheckedInResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var checkInResult = workshop.CheckInRegistration(command.RegistrationId);
        if (!checkInResult.IsSuccess)
            return Result<RegistrationCheckedInResponse>.Failure(checkInResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<RegistrationCheckedInResponse>.Success(
            new RegistrationCheckedInResponse(command.WorkshopId, command.RegistrationId, "CheckedIn"));
    }
}
