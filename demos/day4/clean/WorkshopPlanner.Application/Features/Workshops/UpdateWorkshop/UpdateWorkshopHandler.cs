using FluentValidation;
using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateWorkshop;

public sealed class UpdateWorkshopHandler(
    IWorkshopRepository repository,
    IValidator<UpdateWorkshopCommand> validator)
    : IRequestHandler<UpdateWorkshopCommand, Result<WorkshopUpdatedResponse>>
{
    public async Task<Result<WorkshopUpdatedResponse>> Handle(UpdateWorkshopCommand command, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Result<WorkshopUpdatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<WorkshopUpdatedResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var workshops = await repository.GetAllAsync(ct);
        if (workshops.Any(item => item.Id != command.WorkshopId && item.Title.Equals(command.Title, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<WorkshopUpdatedResponse>.Failure(
                ErrorType.Conflict,
                $"A workshop named '{command.Title}' already exists.");
        }

        var updateResult = workshop.UpdateDetails(command.Title, command.City, command.MaxAttendees);
        if (!updateResult.IsSuccess)
            return Result<WorkshopUpdatedResponse>.Failure(updateResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<WorkshopUpdatedResponse>.Success(
            new WorkshopUpdatedResponse(workshop.Id, workshop.Title, workshop.City, workshop.MaxAttendees));
    }
}
