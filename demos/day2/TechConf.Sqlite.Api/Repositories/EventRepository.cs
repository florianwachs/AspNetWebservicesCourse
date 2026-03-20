using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public class EventRepository(AppDbContext db) : IEventRepository
{
    public async Task<List<EventListItemDto>> GetAllAsync(string? city, CancellationToken cancellationToken = default)
    {
        var query = db.Events.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim().ToLower();
            query = query.Where(e => e.City.ToLower() == normalizedCity);
        }

        return await query
            .OrderBy(e => e.Date)
            .Select(e => new EventListItemDto(
                e.Id,
                e.Name,
                e.Date,
                e.City,
                e.Description,
                e.Sessions.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<EventDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Include + ThenInclude lets students inspect how EF loads a graph in one query.
        var evt = await db.Events
            .AsNoTracking()
            .Include(e => e.Sessions)
                .ThenInclude(s => s.Speakers)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        return MapEventDetail(evt);
    }

    public async Task<List<SessionDto>> GetSessionsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var evt = await db.Events
            .AsNoTracking()
            .Include(e => e.Sessions)
                .ThenInclude(s => s.Speakers)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", eventId);
        }

        return evt.Sessions
            .OrderBy(s => s.Title)
            .Select(MapSession)
            .ToList();
    }

    public async Task<EventListItemDto> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        var evt = new Event
        {
            Name = request.Name,
            Date = request.Date,
            City = request.City,
            Description = request.Description
        };

        db.Events.Add(evt);
        await db.SaveChangesAsync(cancellationToken);

        return new EventListItemDto(evt.Id, evt.Name, evt.Date, evt.City, evt.Description, 0);
    }

    public async Task UpdateAsync(int id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        var evt = await db.Events.FindAsync([id], cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        evt.Name = request.Name;
        evt.Date = request.Date;
        evt.City = request.City;
        evt.Description = request.Description;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var evt = await db.Events.FindAsync([id], cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        db.Events.Remove(evt);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static EventDetailDto MapEventDetail(Event evt) =>
        new(
            evt.Id,
            evt.Name,
            evt.Date,
            evt.City,
            evt.Description,
            evt.Sessions
                .OrderBy(s => s.Title)
                .Select(MapSession)
                .ToList());

    private static SessionDto MapSession(Session session) =>
        new(
            session.Id,
            session.Title,
            session.DurationMinutes,
            session.Speakers
                .OrderBy(sp => sp.Name)
                .Select(sp => sp.Name)
                .ToList());
}
