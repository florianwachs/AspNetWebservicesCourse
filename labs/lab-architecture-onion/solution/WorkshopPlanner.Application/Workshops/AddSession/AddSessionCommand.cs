namespace WorkshopPlanner.Application.Workshops.AddSession;

public sealed record AddSessionCommand(int WorkshopId, string Title, string SpeakerName, int DurationMinutes);

public sealed record AddSessionRequest(string Title, string SpeakerName, int DurationMinutes);
