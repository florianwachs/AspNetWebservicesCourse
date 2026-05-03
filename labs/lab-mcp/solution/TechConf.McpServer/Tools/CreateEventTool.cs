using System.ComponentModel;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;
using TechConf.McpServer.Models;

namespace TechConf.McpServer.Tools;

[McpServerToolType]
public static class CreateEventTool
{
    [McpServerTool(Name = "create_event"), Description("Create a new TechConf event with title, dates, location, and capacity")]
    public static async Task<string> ExecuteAsync(
        AppDbContext db,
        [Description("The title of the event")] string title,
        [Description("Event description")] string? description,
        [Description("Start date in ISO 8601 format")] DateTime startDate,
        [Description("End date in ISO 8601 format")] DateTime endDate,
        [Description("Event location/venue")] string location,
        [Description("Maximum number of attendees")] int maxAttendees,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "❌ Title is required";

        if (endDate <= startDate)
            return "❌ End date must be after start date";

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Location = location,
            MaxAttendees = maxAttendees,
            Status = EventStatus.Draft
        };

        db.Events.Add(ev);
        await db.SaveChangesAsync(ct);
        return $"✅ Event '{title}' created with ID {ev.Id}";
    }
}
