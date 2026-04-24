namespace WorkshopPlanner.Domain.Workshops;

public sealed class WorkshopSession(int id, string title, string speakerName, int durationMinutes)
{
    public int Id { get; } = id;

    public string Title { get; private set; } = title;

    public string SpeakerName { get; private set; } = speakerName;

    public int DurationMinutes { get; private set; } = durationMinutes;

    public void Update(string title, string speakerName, int durationMinutes)
    {
        Title = title;
        SpeakerName = speakerName;
        DurationMinutes = durationMinutes;
    }
}
