using FluentValidation;
using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;

public sealed class CreateWorkshopHandler(
    WorkshopStore store,
    IValidator<CreateWorkshopCommand> validator)
    : IRequestHandler<CreateWorkshopCommand, Result<WorkshopCreatedResponse>>
{
    public async Task<Result<WorkshopCreatedResponse>> Handle(CreateWorkshopCommand request, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<WorkshopCreatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        if (store.Workshops.Any(item => item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
            return Result<WorkshopCreatedResponse>.Failure(
                ErrorType.Conflict,
                $"A workshop named '{request.Title}' already exists.");

        var workshop = new WorkshopState(
            store.GetNextWorkshopId(),
            request.Title.Trim(),
            request.City.Trim(),
            request.MaxAttendees);

        store.Workshops.Add(workshop);

        return Result<WorkshopCreatedResponse>.Success(new WorkshopCreatedResponse(workshop.Id));
    }
}
