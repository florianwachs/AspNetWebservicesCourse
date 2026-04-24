namespace WorkshopPlanner.Domain.Workshops;

public sealed record WorkshopSession(int Id, string Title, string SpeakerName, int DurationMinutes);
