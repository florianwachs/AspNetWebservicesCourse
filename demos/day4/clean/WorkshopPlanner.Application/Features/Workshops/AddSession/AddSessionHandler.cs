using FluentValidation;
using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.AddSession;

public sealed class AddSessionHandler(
    IWorkshopRepository repository,
    IValidator<AddSessionCommand> validator)
    : IRequestHandler<AddSessionCommand, Result<SessionCreatedResponse>>
{
    public async Task<Result<SessionCreatedResponse>> Handle(AddSessionCommand command, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Result<SessionCreatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<SessionCreatedResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var addSessionResult = workshop.AddSession(
            repository.GetNextSessionId(),
            command.Title,
            command.SpeakerName,
            command.DurationMinutes);

        if (!addSessionResult.IsSuccess)
            return Result<SessionCreatedResponse>.Failure(addSessionResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<SessionCreatedResponse>.Success(new SessionCreatedResponse(command.WorkshopId, workshop.Sessions.Max(session => session.Id)));
    }
}
