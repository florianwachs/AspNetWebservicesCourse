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

    // TODO: Task 4 - Refactor all methods below to use AppDbContext instead of in-memory data

    private static async Task<IResult> GetAllEvents(AppDbContext db, string? city, int page = 1, int pageSize = 20)
    {
        // TODO: Query events from database with optional city filter and pagination
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventById(int id, AppDbContext db)
    {
        // TODO: Find event by ID, throw NotFoundException if not found
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventSessions(int id, AppDbContext db)
    {
        // TODO: Get all sessions for an event with speaker names (use Include + ThenInclude)
        throw new NotImplementedException();
    }

    private static async Task<IResult> CreateEvent(CreateEventRequest request, AppDbContext db)
    {
        // TODO: Create event from request, save to database, return Created
        throw new NotImplementedException();
    }

    private static async Task<IResult> UpdateEvent(int id, UpdateEventRequest request, AppDbContext db)
    {
        // TODO: Find and update event, throw NotFoundException if not found
        throw new NotImplementedException();
    }

    private static async Task<IResult> DeleteEvent(int id, AppDbContext db)
    {
        // TODO: Find and delete event, throw NotFoundException if not found
        throw new NotImplementedException();
    }
}
