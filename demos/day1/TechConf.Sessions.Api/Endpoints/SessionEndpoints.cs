using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechConf.Sessions.Api.Data;
using TechConf.Sessions.Api.Models;

namespace TechConf.Sessions.Api.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions")
            .WithTags("Sessions");

        group.MapGet("/", GetAllSessions)
            .WithName("GetSessions")
            .WithSummary("List sessions with optional query-string filters")
            .Produces<List<Session>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", GetSessionById)
            .WithName("GetSessionById")
            .WithSummary("Get one session by id")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateSession)
            .WithName("CreateSession")
            .WithSummary("Create a new session")
            .Accepts<CreateSessionRequest>("application/json")
            .Produces<Session>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", ReplaceSession)
            .WithName("ReplaceSession")
            .WithSummary("Replace an existing session")
            .Accepts<UpdateSessionRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("/{id:int}", PatchSession)
            .WithName("PatchSession")
            .WithSummary("Partially update an existing session")
            .Accepts<PatchSessionRequest>("application/merge-patch+json")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", DeleteSession)
            .WithName("DeleteSession")
            .WithSummary("Delete a session")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Ok<List<Session>> GetAllSessions(
        ISessionRepository repository,
        string? track,
        string? speaker,
        bool? published,
        string? search)
    {
        var query = repository.List().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(track))
        {
            query = query.Where(session => session.Track.Equals(track, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(speaker))
        {
            query = query.Where(session => session.Speaker.Equals(speaker, StringComparison.OrdinalIgnoreCase));
        }

        if (published is not null)
        {
            query = query.Where(session => session.IsPublished == published);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(session =>
                session.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                session.Speaker.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                session.Track.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return TypedResults.Ok(query.ToList());
    }

    private static Results<Ok<Session>, NotFound> GetSessionById(int id, ISessionRepository repository)
    {
        var session = repository.GetById(id);

        return session is not null
            ? TypedResults.Ok(session)
            : TypedResults.NotFound();
    }

    private static Results<CreatedAtRoute<Session>, UnprocessableEntity<HttpValidationProblemDetails>, ProblemHttpResult> CreateSession(
        CreateSessionRequest request,
        ISessionRepository repository)
    {
        var normalizedTitle = request.Title.Trim();
        var normalizedSpeaker = request.Speaker.Trim();
        var normalizedTrack = request.Track.Trim();

        var errors = ValidateRequest(
            normalizedTitle,
            normalizedSpeaker,
            normalizedTrack,
            request.StartsAt,
            request.DurationMinutes);

        if (errors.Count > 0)
        {
            return TypedResults.UnprocessableEntity(new HttpValidationProblemDetails(errors));
        }

        if (repository.ExistsWithTitle(normalizedTitle))
        {
            return DuplicateTitleProblem(normalizedTitle);
        }

        var session = new Session(
            repository.GetNextId(),
            normalizedTitle,
            normalizedSpeaker,
            normalizedTrack,
            request.StartsAt,
            request.DurationMinutes,
            request.IsPublished);

        repository.Add(session);

        return TypedResults.CreatedAtRoute(
            session,
            routeName: "GetSessionById",
            routeValues: new { id = session.Id });
    }

    private static Results<NoContent, NotFound, UnprocessableEntity<HttpValidationProblemDetails>, ProblemHttpResult> ReplaceSession(
        int id,
        UpdateSessionRequest request,
        ISessionRepository repository)
    {
        if (repository.GetById(id) is null)
        {
            return TypedResults.NotFound();
        }

        var normalizedTitle = request.Title.Trim();
        var normalizedSpeaker = request.Speaker.Trim();
        var normalizedTrack = request.Track.Trim();

        var errors = ValidateRequest(
            normalizedTitle,
            normalizedSpeaker,
            normalizedTrack,
            request.StartsAt,
            request.DurationMinutes);

        if (errors.Count > 0)
        {
            return TypedResults.UnprocessableEntity(new HttpValidationProblemDetails(errors));
        }

        if (repository.ExistsWithTitle(normalizedTitle, id))
        {
            return DuplicateTitleProblem(normalizedTitle);
        }

        var updatedSession = new Session(
            id,
            normalizedTitle,
            normalizedSpeaker,
            normalizedTrack,
            request.StartsAt,
            request.DurationMinutes,
            request.IsPublished);

        if (!repository.Update(updatedSession))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    private static Results<Ok<Session>, NotFound, UnprocessableEntity<HttpValidationProblemDetails>, ProblemHttpResult> PatchSession(
        int id,
        PatchSessionRequest request,
        ISessionRepository repository)
    {
        var existingSession = repository.GetById(id);

        if (existingSession is null)
        {
            return TypedResults.NotFound();
        }

        var updatedSession = existingSession with
        {
            Title = request.Title ?? existingSession.Title,
            Speaker = request.Speaker ?? existingSession.Speaker,
            Track = request.Track ?? existingSession.Track,
            StartsAt = request.StartsAt ?? existingSession.StartsAt,
            DurationMinutes = request.DurationMinutes ?? existingSession.DurationMinutes,
            IsPublished = request.IsPublished ?? existingSession.IsPublished
        };

        var normalizedSession = updatedSession with
        {
            Title = updatedSession.Title.Trim(),
            Speaker = updatedSession.Speaker.Trim(),
            Track = updatedSession.Track.Trim()
        };

        var errors = ValidateRequest(
            normalizedSession.Title,
            normalizedSession.Speaker,
            normalizedSession.Track,
            normalizedSession.StartsAt,
            normalizedSession.DurationMinutes);

        if (errors.Count > 0)
        {
            return TypedResults.UnprocessableEntity(new HttpValidationProblemDetails(errors));
        }

        if (repository.ExistsWithTitle(normalizedSession.Title, id))
        {
            return DuplicateTitleProblem(normalizedSession.Title);
        }

        if (!repository.Update(normalizedSession))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(normalizedSession);
    }

    private static Results<NoContent, NotFound> DeleteSession(int id, ISessionRepository repository)
    {
        return repository.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    private static Dictionary<string, string[]> ValidateRequest(
        string title,
        string speaker,
        string track,
        DateTime startsAt,
        int durationMinutes)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(title))
        {
            errors[nameof(CreateSessionRequest.Title)] = ["Title is required."];
        }
        else if (title.Length > 120)
        {
            errors[nameof(CreateSessionRequest.Title)] = ["Title must be 120 characters or fewer."];
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            errors[nameof(CreateSessionRequest.Speaker)] = ["Speaker is required."];
        }

        if (string.IsNullOrWhiteSpace(track))
        {
            errors[nameof(CreateSessionRequest.Track)] = ["Track is required."];
        }

        if (startsAt == default)
        {
            errors[nameof(CreateSessionRequest.StartsAt)] = ["StartsAt must be a valid date and time."];
        }

        if (durationMinutes is < 15 or > 240)
        {
            errors[nameof(CreateSessionRequest.DurationMinutes)] =
                ["DurationMinutes must be between 15 and 240 minutes."];
        }

        return errors;
    }

    private static ProblemHttpResult DuplicateTitleProblem(string title) =>
        TypedResults.Problem(
            title: "Conflict",
            detail: $"A session with the title '{title}' already exists.",
            statusCode: StatusCodes.Status409Conflict);
}
