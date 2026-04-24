using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Domain.Common;
using WorkshopPlanner.Domain.Workshops;

namespace WorkshopPlanner.Infrastructure.Repositories;

public sealed class InMemoryWorkshopRepository : IWorkshopRepository
{
    private readonly List<Workshop> _workshops = BuildSeedData();
    private int _nextWorkshopId = 3;
    private int _nextSessionId = 5;
    private int _nextRegistrationId = 7;

    public Task<IReadOnlyCollection<Workshop>> GetAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyCollection<Workshop>>(_workshops);

    public Task<Workshop?> GetByIdAsync(int id, CancellationToken ct) =>
        Task.FromResult(_workshops.FirstOrDefault(workshop => workshop.Id == id));

    public Task<bool> ExistsByTitleAsync(string title, CancellationToken ct) =>
        Task.FromResult(_workshops.Any(workshop => workshop.Title.Equals(title, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Workshop workshop, CancellationToken ct)
    {
        _workshops.Add(workshop);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;

    public int GetNextWorkshopId() => _nextWorkshopId++;

    public int GetNextSessionId() => _nextSessionId++;

    public int GetNextRegistrationId() => _nextRegistrationId++;

    private static List<Workshop> BuildSeedData()
    {
        var apiDesign = EnsureSuccess(Workshop.Create(1, "API Design Bootcamp", "Munich", 5));
        EnsureSuccess(apiDesign.AddSession(1, "Minimal APIs that stay readable", "Nina Weber", 90));
        EnsureSuccess(apiDesign.AddSession(2, "When CRUD stops being simple", "Jonas Hartmann", 75));
        EnsureSuccess(apiDesign.Publish(DateTime.UtcNow.AddDays(-7)));
        EnsureSuccessRegistration(apiDesign.AddRegistration(1, "Sofia Mayer", "sofia@demo.dev"));
        EnsureSuccessRegistration(apiDesign.AddRegistration(2, "Milan Beck", "milan@demo.dev"));
        EnsureSuccessRegistration(apiDesign.AddRegistration(3, "Tara Wolf", "tara@demo.dev"));
        EnsureSuccessRegistration(apiDesign.AddRegistration(4, "Noah Winter", "noah@demo.dev"));
        EnsureSuccessRegistration(apiDesign.AddRegistration(5, "Emma Kurz", "emma@demo.dev"));
        EnsureSuccessRegistration(apiDesign.AddRegistration(6, "Lina Vogt", "lina@demo.dev"));

        var architecture = EnsureSuccess(Workshop.Create(2, "Architecture Refactoring Clinic", "Rosenheim", 24));
        EnsureSuccess(architecture.AddSession(3, "Finding the pain in a layered baseline", "Florian Koch", 60));
        EnsureSuccess(architecture.AddSession(4, "How to split features without over-engineering", "Mira Adler", 60));

        return [apiDesign, architecture];
    }

    private static Workshop EnsureSuccess(Result<Workshop> result)
    {
        if (!result.IsSuccess || result.Value is null)
            throw new InvalidOperationException(result.Error?.Message ?? "Unexpected workshop seed failure.");

        return result.Value;
    }

    private static void EnsureSuccessRegistration(Result<WorkshopRegistration> result)
    {
        if (!result.IsSuccess || result.Value is null)
            throw new InvalidOperationException(result.Error?.Message ?? "Unexpected registration seed failure.");
    }

    private static void EnsureSuccess(Result result)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.Error?.Message ?? "Unexpected workshop seed failure.");
    }
}
