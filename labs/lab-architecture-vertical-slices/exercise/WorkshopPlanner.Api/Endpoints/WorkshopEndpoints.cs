using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Models;

namespace WorkshopPlanner.Api.Endpoints;

public static class WorkshopEndpoints
{
    public static void MapWorkshopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workshops").WithTags("Workshops");

        group.MapGet("/", (WorkshopStore store) =>
            TypedResults.Ok(store.Workshops.Select(workshop => new WorkshopSummaryResponse(
                workshop.Id,
                workshop.Title,
                workshop.City,
                workshop.MaxAttendees,
                workshop.Sessions.Count,
                workshop.PublishedOnUtc is not null))));

        group.MapGet("/{id:int}", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            return workshop is not null
                ? Results.Ok(workshop)
                : Results.NotFound();
        });

        group.MapPost("/", (CreateWorkshopRequest request, WorkshopStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Title is required." });

            if (string.IsNullOrWhiteSpace(request.City))
                return Results.BadRequest(new { error = "City is required." });

            if (request.MaxAttendees < 5)
                return Results.BadRequest(new { error = "Max attendees must be at least 5." });

            if (store.Workshops.Any(item => item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A workshop named '{request.Title}' already exists." });

            var workshop = new Workshop(
                store.GetNextWorkshopId(),
                request.Title.Trim(),
                request.City.Trim(),
                request.MaxAttendees);

            store.Workshops.Add(workshop);

            return Results.Created($"/api/workshops/{workshop.Id}", workshop);
        });

        group.MapPost("/{id:int}/sessions", (int id, AddSessionRequest request, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound();

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "Published workshops cannot be changed." });

            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Session title is required." });

            if (string.IsNullOrWhiteSpace(request.SpeakerName))
                return Results.BadRequest(new { error = "Speaker name is required." });

            if (request.DurationMinutes is < 30 or > 180)
                return Results.BadRequest(new { error = "Session duration must be between 30 and 180 minutes." });

            if (workshop.Sessions.Any(session => session.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A session named '{request.Title}' already exists in this workshop." });

            var totalDuration = workshop.Sessions.Sum(session => session.DurationMinutes) + request.DurationMinutes;
            if (totalDuration > 480)
                return Results.BadRequest(new { error = "A workshop cannot exceed 480 total session minutes." });

            var session = new WorkshopSession(
                store.GetNextSessionId(),
                request.Title.Trim(),
                request.SpeakerName.Trim(),
                request.DurationMinutes);

            workshop.Sessions.Add(session);

            return Results.Created($"/api/workshops/{workshop.Id}", new { workshopId = workshop.Id, sessionId = session.Id });
        });

        group.MapPost("/{id:int}/publish", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound();

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "The workshop is already published." });

            if (workshop.Sessions.Count == 0)
                return Results.BadRequest(new { error = "Add at least one session before publishing." });

            if (workshop.Sessions.Sum(session => session.DurationMinutes) < 60)
                return Results.BadRequest(new { error = "A published workshop needs at least 60 total minutes of sessions." });

            workshop.PublishedOnUtc = DateTime.UtcNow;

            return Results.Ok(new
            {
                workshop.Id,
                workshop.Title,
                Status = "Published",
                workshop.PublishedOnUtc
            });
        });
    }
}
