using TechConf.Api.Data;
using TechConf.Api.Exceptions;
using TechConf.Api.Models;
using TechConf.Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events");

        group.MapGet("/", GetAllEvents);
        group.MapGet("/{id:int}", GetEventById);
        group.MapGet("/{id:int}/sessions", GetEventSessions);
        group.MapPost("/", CreateEvent)
            .AddEndpointFilter<ValidationFilter<CreateEventRequest>>();
        group.MapPut("/{id:int}", UpdateEvent);
        group.MapDelete("/{id:int}", DeleteEvent);
    }

    private static async Task<Ok<List<EventDto>>> GetAllEvents(
        AppDbContext db, string? city, int page = 1, int pageSize = 20)
    {
        var query = db.Events.AsQueryable();

        if (city is not null)
            query = query.Where(e => EF.Functions.ILike(e.City, city));

        var events = await query
            .OrderBy(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventDto(e.Id, e.Name, e.Date, e.City, e.Description, e.Sessions.Count))
            .ToListAsync();

        return TypedResults.Ok(events);
    }

    private static async Task<Results<Ok<EventDetailDto>, NotFound>> GetEventById(int id, AppDbContext db)
    {
        var evt = await db.Events
            .Include(e => e.Sessions)
                .ThenInclude(s => s.Speakers)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evt is null) return TypedResults.NotFound();

        var dto = new EventDetailDto(
            evt.Id, evt.Name, evt.Date, evt.City, evt.Description,
            evt.Sessions.Select(s => new SessionDto(
                s.Id, s.Title, s.Duration,
                s.Speakers.Select(sp => sp.Name).ToList())).ToList());

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Ok<List<SessionDto>>, NotFound>> GetEventSessions(int id, AppDbContext db)
    {
        var evt = await db.Events
            .Include(e => e.Sessions)
                .ThenInclude(s => s.Speakers)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evt is null) return TypedResults.NotFound();

        var sessions = evt.Sessions.Select(s => new SessionDto(
            s.Id, s.Title, s.Duration,
            s.Speakers.Select(sp => sp.Name).ToList())).ToList();

        return TypedResults.Ok(sessions);
    }

    private static async Task<Created<EventDto>> CreateEvent(CreateEventRequest request, AppDbContext db)
    {
        var evt = new Event
        {
            Name = request.Name,
            Date = request.Date,
            City = request.City,
            Description = request.Description
        };

        db.Events.Add(evt);
        await db.SaveChangesAsync();

        var dto = new EventDto(evt.Id, evt.Name, evt.Date, evt.City, evt.Description, 0);
        return TypedResults.Created($"/api/events/{evt.Id}", dto);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateEvent(
        int id, UpdateEventRequest request, AppDbContext db)
    {
        var evt = await db.Events.FindAsync(id);
        if (evt is null) return TypedResults.NotFound();

        evt.Name = request.Name;
        evt.Date = request.Date;
        evt.City = request.City;
        evt.Description = request.Description;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteEvent(int id, AppDbContext db)
    {
        var evt = await db.Events.FindAsync(id);
        if (evt is null) return TypedResults.NotFound();

        db.Events.Remove(evt);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
