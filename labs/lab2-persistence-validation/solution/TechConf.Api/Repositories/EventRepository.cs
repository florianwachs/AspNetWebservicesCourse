using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _db;

    public EventRepository(AppDbContext db) => _db = db;

    public async Task<List<EventDto>> GetAllAsync(
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var normalizedCity = city?.Trim();
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Events
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(normalizedCity))
        {
            query = query.Where(e => EF.Functions.ILike(e.City, normalizedCity));
        }

        return await query
            .OrderBy(e => e.Date)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .Select(e => new EventDto(
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
        var evt = await _db.Events
            .AsNoTracking()
            .Include(e => e.Sessions)
                .ThenInclude(s => s.Speakers)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        return new EventDetailDto(
            evt.Id,
            evt.Name,
            evt.Date,
            evt.City,
            evt.Description,
            evt.Sessions
                .OrderBy(s => s.Title)
                .Select(MapSession)
                .ToList());
    }

    public async Task<List<SessionDto>> GetSessionsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var evt = await _db.Events
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

    public async Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        var evt = new Event
        {
            Name = request.Name,
            Date = request.Date,
            City = request.City,
            Description = request.Description
        };

        _db.Events.Add(evt);
        await _db.SaveChangesAsync(cancellationToken);

        return new EventDto(evt.Id, evt.Name, evt.Date, evt.City, evt.Description, 0);
    }

    public async Task UpdateAsync(int id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        var evt = await _db.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        evt.Name = request.Name;
        evt.Date = request.Date;
        evt.City = request.City;
        evt.Description = request.Description;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var evt = await _db.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (evt is null)
        {
            throw new NotFoundException("Event", id);
        }

        _db.Events.Remove(evt);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static SessionDto MapSession(Session session) =>
        new(
            session.Id,
            session.Title,
            session.Duration,
            session.Speakers
                .OrderBy(sp => sp.Name)
                .Select(sp => sp.Name)
                .ToList());
}
