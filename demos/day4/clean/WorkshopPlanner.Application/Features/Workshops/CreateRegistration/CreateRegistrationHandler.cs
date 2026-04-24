using FluentValidation;
using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CreateRegistration;

public sealed class CreateRegistrationHandler(
    IWorkshopRepository repository,
    IValidator<CreateRegistrationCommand> validator)
    : IRequestHandler<CreateRegistrationCommand, Result<RegistrationCreatedResponse>>
{
    public async Task<Result<RegistrationCreatedResponse>> Handle(CreateRegistrationCommand command, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Result<RegistrationCreatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<RegistrationCreatedResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var registrationResult = workshop.AddRegistration(
            repository.GetNextRegistrationId(),
            command.AttendeeName,
            command.AttendeeEmail);

        if (!registrationResult.IsSuccess)
            return Result<RegistrationCreatedResponse>.Failure(registrationResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<RegistrationCreatedResponse>.Success(
            new RegistrationCreatedResponse(
                command.WorkshopId,
                registrationResult.Value!.Id,
                registrationResult.Value.Status.ToString()));
    }
}
