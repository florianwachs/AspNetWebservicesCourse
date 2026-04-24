using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.AddSession;

public sealed class AddSessionHandler(WorkshopStore store)
    : IRequestHandler<AddSessionCommand, Result<SessionCreatedResponse>>
{
    public Task<Result<SessionCreatedResponse>> Handle(AddSessionCommand request, CancellationToken ct)
    {
        var workshop = store.Workshops.FirstOrDefault(item => item.Id == request.WorkshopId);
        if (workshop is null)
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(ErrorType.NotFound, $"Workshop {request.WorkshopId} was not found."));

        if (workshop.IsPublished)
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(ErrorType.Conflict, "Published workshops cannot be changed."));

        if (workshop.Sessions.Any(session => session.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(
                    ErrorType.Conflict,
                    $"A session named '{request.Title}' already exists in this workshop."));

        var totalDuration = workshop.Sessions.Sum(session => session.DurationMinutes) + request.DurationMinutes;
        if (totalDuration > 480)
            return Task.FromResult(
                Result<SessionCreatedResponse>.Failure(
                    ErrorType.Validation,
                    "A workshop cannot exceed 480 total session minutes."));

        var sessionId = store.GetNextSessionId();
        workshop.Sessions.Add(new SessionState(sessionId, request.Title.Trim(), request.SpeakerName.Trim(), request.DurationMinutes));

        return Task.FromResult(Result<SessionCreatedResponse>.Success(new SessionCreatedResponse(request.WorkshopId, sessionId)));
    }
}
