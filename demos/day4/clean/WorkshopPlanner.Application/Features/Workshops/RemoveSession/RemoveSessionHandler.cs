using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.RemoveSession;

public sealed class RemoveSessionHandler(IWorkshopRepository repository)
    : IRequestHandler<RemoveSessionCommand, Result<SessionRemovedResponse>>
{
    public async Task<Result<SessionRemovedResponse>> Handle(RemoveSessionCommand command, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(command.WorkshopId, ct);
        if (workshop is null)
            return Result<SessionRemovedResponse>.Failure(ErrorType.NotFound, $"Workshop {command.WorkshopId} was not found.");

        var removeResult = workshop.RemoveSession(command.SessionId);
        if (!removeResult.IsSuccess)
            return Result<SessionRemovedResponse>.Failure(removeResult.Error!);

        await repository.SaveChangesAsync(ct);

        return Result<SessionRemovedResponse>.Success(new SessionRemovedResponse(command.WorkshopId, command.SessionId));
    }
}
