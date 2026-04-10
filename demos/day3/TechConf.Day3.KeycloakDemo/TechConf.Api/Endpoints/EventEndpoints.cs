using TechConf.Api.Data;
using TechConf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events");

        group.MapGet("/", async (AppDbContext db) =>
            TypedResults.Ok(await db.Events
                .Select(e => new EventDto(e.Id, e.Name, e.Date, e.City, e.Description))
                .ToListAsync()));

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var evt = await db.Events.FindAsync(id);
            return evt is not null
                ? Results.Ok(new EventDto(evt.Id, evt.Name, evt.Date, evt.City, evt.Description))
                : Results.NotFound();
        });

        group.MapPost("/", async (CreateEventRequest request, AppDbContext db) =>
        {
            var evt = new Event
            {
                Name = request.Name,
                Date = NormalizeToUtc(request.Date),
                City = request.City,
                Description = request.Description
            };
            db.Events.Add(evt);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/events/{evt.Id}",
                new EventDto(evt.Id, evt.Name, evt.Date, evt.City, evt.Description));
        }).RequireAuthorization("Organizer");

        group.MapPut("/{id:int}", async (int id, UpdateEventRequest request, AppDbContext db) =>
        {
            var evt = await db.Events.FindAsync(id);
            if (evt is null) return Results.NotFound();
            evt.Name = request.Name;
            evt.Date = NormalizeToUtc(request.Date);
            evt.City = request.City;
            evt.Description = request.Description;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("Organizer");

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var evt = await db.Events.FindAsync(id);
            if (evt is null) return Results.NotFound();
            db.Events.Remove(evt);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("Organizer");
    }

    private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
