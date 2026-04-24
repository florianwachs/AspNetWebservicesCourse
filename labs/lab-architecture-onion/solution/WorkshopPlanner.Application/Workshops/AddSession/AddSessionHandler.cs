using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Workshops.AddSession;

public sealed class AddSessionHandler(IWorkshopRepository repository)
{
    public async Task<Result<SessionCreatedResponse>> Handle(AddSessionCommand command, CancellationToken ct)
    {
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
