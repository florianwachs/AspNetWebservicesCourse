using System.ComponentModel;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;
using TechConf.McpServer.Models;

namespace TechConf.McpServer.Tools;

// TODO: Task 3 — Implement the create_event tool
//
// This tool lets AI assistants create new TechConf events.
//
// Requirements:
// 1. Decorate the class with [McpServerToolType]
// 2. Create a static async method with [McpServerTool(Name = "create_event")] and
//    [Description("Create a new TechConf event with title, dates, location, and capacity")]
// 3. Parameters (each with [Description]):
//    - AppDbContext db
//    - string title — "The title of the event"
//    - string? description — "Event description"
//    - DateTime startDate — "Start date in ISO 8601 format"
//    - DateTime endDate — "End date in ISO 8601 format"
//    - string location — "Event location/venue"
//    - int maxAttendees — "Maximum number of attendees"
//    - CancellationToken ct
// 4. Implementation:
//    - Validate: title must not be empty, endDate must be after startDate
//    - Create Event with Status = Draft
//    - Return success message with the new event ID

public static class CreateEventTool
{
    public static Task<string> ExecuteAsync()
    {
        throw new NotImplementedException("Implement create_event tool");
    }
}
