using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAll(
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<Event> GetById(int id, CancellationToken cancellationToken = default);
    Task<List<Session>> GetSessions(int eventId, CancellationToken cancellationToken = default);
    Task<Event> Create(CreateEventRequest request, CancellationToken cancellationToken = default);
    Task Update(int id, UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task Delete(int id, CancellationToken cancellationToken = default);
}
