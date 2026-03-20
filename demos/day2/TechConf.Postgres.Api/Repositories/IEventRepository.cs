using TechConf.Api.Models;

namespace TechConf.Api.Repositories;

public interface IEventRepository
{
    Task<List<EventListItemDto>> GetAllAsync(string? city, CancellationToken cancellationToken = default);
    Task<EventDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SessionDto>> GetSessionsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<EventListItemDto> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
