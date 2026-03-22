using TechConf.Api.Data;
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
        // TODO: Task 3 - Query events with optional city filtering and pagination.
        // Project the result to EventDto and include the session count.
        throw new NotImplementedException();
    }

    public async Task<EventDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Load one event including sessions and speakers with Include/ThenInclude.
        // Throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task<List<SessionDto>> GetSessionsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Load one event including sessions and speakers with eager loading.
        // Map the sessions to SessionDto and throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Create an Event entity from the request, save it,
        // and return an EventDto for the created event.
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Find the event, update its properties, save changes,
        // and throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // TODO: Task 3 - Find the event, remove it, save changes,
        // and throw NotFoundException when the event does not exist.
        throw new NotImplementedException();
    }
}
