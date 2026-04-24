namespace WorkshopPlanner.Application.Workshops.CreateWorkshop;

public sealed record CreateWorkshopCommand(string Title, string City, int MaxAttendees);
