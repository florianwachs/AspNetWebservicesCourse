using TechConf.Api.Data;
using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _db;

    public EventRepository(AppDbContext db) => _db = db;

    public async Task<List<Event>> GetAll(
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Query events with optional city filtering and pagination.
        // Return Event entities, and include Sessions so the endpoint can map SessionCount
        // without triggering extra queries.
        throw new NotImplementedException();
    }

    public async Task<Event> GetById(int id, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Load one event including sessions and speakers with Include/ThenInclude.
        // Throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task<List<Session>> GetSessions(int eventId, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Load one event including sessions and speakers with eager loading.
        // Return Session entities with Speakers loaded, and throw NotFoundException
        // when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task<Event> Create(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Create an Event entity from the request, save it,
        // and return the created Event entity.
        throw new NotImplementedException();
    }

    public async Task Update(int id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Find the event, update its properties, save changes,
        // and throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task Delete(int id, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Find the event, remove it, save changes,
        // and throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }
}
