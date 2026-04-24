using WorkshopPlanner.Domain.Workshops;

namespace WorkshopPlanner.Application.Abstractions;

public interface IWorkshopRepository
{
    Task<IReadOnlyCollection<Workshop>> GetAllAsync(CancellationToken ct);

    Task<Workshop?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> ExistsByTitleAsync(string title, CancellationToken ct);

    Task AddAsync(Workshop workshop, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);

    int GetNextWorkshopId();

    int GetNextSessionId();

    int GetNextRegistrationId();
}
