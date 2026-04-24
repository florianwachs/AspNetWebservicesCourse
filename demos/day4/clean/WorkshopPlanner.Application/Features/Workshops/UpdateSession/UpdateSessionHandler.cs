using FluentValidation;
using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateSession;

public sealed class UpdateSessionHandler(
    IWorkshopRepository repository,
    IValidator<UpdateSessionCommand> validator)
    : IRequestHandler<UpdateSessionCommand, Result<SessionUpdatedResponse>>
{
    public async Task<Result<SessionUpdatedResponse>> Handle(UpdateSessionCommand command, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Result<SessionUpdatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<SessionUpdatedResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var updateResult = workshop.UpdateSession(command.SessionId, command.Title, command.SpeakerName, command.DurationMinutes);
        if (!updateResult.IsSuccess)
            return Result<SessionUpdatedResponse>.Failure(updateResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<SessionUpdatedResponse>.Success(new SessionUpdatedResponse(command.WorkshopId, command.SessionId));
    }
}
