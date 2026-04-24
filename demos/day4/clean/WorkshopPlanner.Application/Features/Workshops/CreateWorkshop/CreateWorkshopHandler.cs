using FluentValidation;
using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;
using WorkshopPlanner.Domain.Workshops;

namespace WorkshopPlanner.Application.Features.Workshops.CreateWorkshop;

public sealed class CreateWorkshopHandler(
    IWorkshopRepository repository,
    IValidator<CreateWorkshopCommand> validator)
    : IRequestHandler<CreateWorkshopCommand, Result<WorkshopCreatedResponse>>
{
    public async Task<Result<WorkshopCreatedResponse>> Handle(CreateWorkshopCommand command, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Result<WorkshopCreatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        var workshopResult = Workshop.Create(
            repository.GetNextWorkshopId(),
            command.Title,
            command.City,
            command.MaxAttendees);

        if (!workshopResult.IsSuccess)
            return Result<WorkshopCreatedResponse>.Failure(workshopResult.Error!);

        if (await repository.ExistsByTitleAsync(workshopResult.Value!.Title, ct))
            return Result<WorkshopCreatedResponse>.Failure(
                ErrorType.Conflict,
                $"A workshop named '{workshopResult.Value.Title}' already exists.");

        await repository.AddAsync(workshopResult.Value!, ct);
        await repository.SaveChangesAsync(ct);

        return Result<WorkshopCreatedResponse>.Success(new WorkshopCreatedResponse(workshopResult.Value!.Id));
    }
}
