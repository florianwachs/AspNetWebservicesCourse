using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Data;

public sealed class WorkshopStore
{
    private int _nextWorkshopId = 3;
    private int _nextSessionId = 5;

    public WorkshopStore()
    {
        var apiDesign = new WorkshopState(1, "API Design Bootcamp", "Munich", 40)
        {
            PublishedOnUtc = DateTime.UtcNow.AddDays(-7)
        };
        apiDesign.Sessions.Add(new SessionState(1, "Minimal APIs that stay readable", "Nina Weber", 90));
        apiDesign.Sessions.Add(new SessionState(2, "When CRUD stops being simple", "Jonas Hartmann", 75));

        var architecture = new WorkshopState(2, "Architecture Refactoring Clinic", "Rosenheim", 24);
        architecture.Sessions.Add(new SessionState(3, "Finding the pain in a layered baseline", "Florian Koch", 60));
        architecture.Sessions.Add(new SessionState(4, "How to split features without over-engineering", "Mira Adler", 60));

        Workshops.Add(apiDesign);
        Workshops.Add(architecture);
    }

    public List<WorkshopState> Workshops { get; } = [];

    public int GetNextWorkshopId() => _nextWorkshopId++;

    public int GetNextSessionId() => _nextSessionId++;
}
