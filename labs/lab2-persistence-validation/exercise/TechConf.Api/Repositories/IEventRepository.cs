using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public interface IEventRepository
{
    Task<List<EventDto>> GetAllAsync(
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<EventDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SessionDto>> GetSessionsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
