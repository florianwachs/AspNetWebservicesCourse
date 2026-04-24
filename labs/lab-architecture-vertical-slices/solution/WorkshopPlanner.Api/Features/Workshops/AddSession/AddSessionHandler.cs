using FluentValidation;
using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.AddSession;

public sealed class AddSessionHandler(
    WorkshopStore store,
    IValidator<AddSessionCommand> validator)
    : IRequestHandler<AddSessionCommand, Result<SessionCreatedResponse>>
{
    public async Task<Result<SessionCreatedResponse>> Handle(AddSessionCommand request, CancellationToken ct)
    {
        var workshop = store.Workshops.FirstOrDefault(item => item.Id == request.WorkshopId);
        if (workshop is null)
            return Result<SessionCreatedResponse>.Failure(ErrorType.NotFound, $"Workshop {request.WorkshopId} was not found.");

        if (workshop.IsPublished)
            return Result<SessionCreatedResponse>.Failure(ErrorType.Conflict, "Published workshops cannot be changed.");

        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<SessionCreatedResponse>.Failure(ErrorType.Validation, validation.Errors[0].ErrorMessage);

        if (workshop.Sessions.Any(session => session.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
            return Result<SessionCreatedResponse>.Failure(
                ErrorType.Conflict,
                $"A session named '{request.Title}' already exists in this workshop.");

        var totalDuration = workshop.Sessions.Sum(session => session.DurationMinutes) + request.DurationMinutes;
        if (totalDuration > 480)
            return Result<SessionCreatedResponse>.Failure(
                ErrorType.Validation,
                "A workshop cannot exceed 480 total session minutes.");

        var sessionId = store.GetNextSessionId();
        workshop.Sessions.Add(new SessionState(sessionId, request.Title.Trim(), request.SpeakerName.Trim(), request.DurationMinutes));

        return Result<SessionCreatedResponse>.Success(new SessionCreatedResponse(request.WorkshopId, sessionId));
    }
}
