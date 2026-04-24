using WorkshopPlanner.Api.Models;

namespace WorkshopPlanner.Api.Data;

public sealed class WorkshopStore
{
    private int _nextWorkshopId = 3;
    private int _nextSessionId = 5;
    private int _nextRegistrationId = 7;

    public WorkshopStore()
    {
        var apiDesign = new Workshop(1, "API Design Bootcamp", "Munich", 5);
        apiDesign.Sessions.Add(new WorkshopSession(1, "Minimal APIs that stay readable", "Nina Weber", 90));
        apiDesign.Sessions.Add(new WorkshopSession(2, "When CRUD stops being simple", "Jonas Hartmann", 75));
        apiDesign.PublishedOnUtc = DateTime.UtcNow.AddDays(-7);
        apiDesign.Registrations.Add(new WorkshopRegistration(1, "Sofia Mayer", "sofia@demo.dev", RegistrationStatus.Confirmed));
        apiDesign.Registrations.Add(new WorkshopRegistration(2, "Milan Beck", "milan@demo.dev", RegistrationStatus.Confirmed));
        apiDesign.Registrations.Add(new WorkshopRegistration(3, "Tara Wolf", "tara@demo.dev", RegistrationStatus.Confirmed));
        apiDesign.Registrations.Add(new WorkshopRegistration(4, "Noah Winter", "noah@demo.dev", RegistrationStatus.Confirmed));
        apiDesign.Registrations.Add(new WorkshopRegistration(5, "Emma Kurz", "emma@demo.dev", RegistrationStatus.Confirmed));
        apiDesign.Registrations.Add(new WorkshopRegistration(6, "Lina Vogt", "lina@demo.dev", RegistrationStatus.Waitlisted));

        var architecture = new Workshop(2, "Architecture Refactoring Clinic", "Rosenheim", 24);
        architecture.Sessions.Add(new WorkshopSession(3, "Finding the pain in a layered baseline", "Florian Koch", 60));
        architecture.Sessions.Add(new WorkshopSession(4, "How to split features without over-engineering", "Mira Adler", 60));

        Workshops.Add(apiDesign);
        Workshops.Add(architecture);
    }

    public List<Workshop> Workshops { get; } = [];

    public int GetNextWorkshopId() => _nextWorkshopId++;

    public int GetNextSessionId() => _nextSessionId++;

    public int GetNextRegistrationId() => _nextRegistrationId++;
}
