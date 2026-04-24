using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.GetSessions;

public sealed class GetSessionsHandler(IWorkshopRepository repository)
    : IRequestHandler<GetSessionsQuery, Result<IReadOnlyList<WorkshopSessionResponse>>>
{
    public async Task<Result<IReadOnlyList<WorkshopSessionResponse>>> Handle(GetSessionsQuery query, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(query.WorkshopId, ct);
        if (workshop is null)
            return Result<IReadOnlyList<WorkshopSessionResponse>>.Failure(ErrorType.NotFound, $"Workshop {query.WorkshopId} was not found.");

        IReadOnlyList<WorkshopSessionResponse> sessions = workshop.Sessions
            .OrderBy(session => session.Title)
            .Select(session => new WorkshopSessionResponse(
                session.Id,
                session.Title,
                session.SpeakerName,
                session.DurationMinutes))
            .ToArray();

        return Result<IReadOnlyList<WorkshopSessionResponse>>.Success(sessions);
    }
}
