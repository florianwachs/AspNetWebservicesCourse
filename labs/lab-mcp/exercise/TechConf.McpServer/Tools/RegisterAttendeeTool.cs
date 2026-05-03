using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;
using TechConf.McpServer.Models;

namespace TechConf.McpServer.Tools;

// TODO: Task 4 — Implement the register_attendee tool
//
// This tool lets AI assistants register attendees for events.
//
// Requirements:
// 1. Decorate the class with [McpServerToolType]
// 2. Create a static async method with [McpServerTool(Name = "register_attendee")] and
//    [Description("Register an attendee for a TechConf event. Checks capacity limits.")]
// 3. Parameters (each with [Description]):
//    - AppDbContext db
//    - Guid eventId — "The event ID to register for"
//    - string name — "Attendee's full name"
//    - string email — "Attendee's email address"
//    - CancellationToken ct
// 4. Implementation:
//    - Find the event (include Registrations)
//    - If not found: return "❌ Event with ID {eventId} not found"
//    - If full: return "❌ Event '{title}' is full ({max} max)"
//    - If already registered (by email): return "⚠️ {email} is already registered"
//    - Otherwise: create or find Attendee, add Registration, save
//    - Return "✅ {name} registered for '{title}'"

public static class RegisterAttendeeTool
{
    public static Task<string> ExecuteAsync()
    {
        throw new NotImplementedException("Implement register_attendee tool");
    }
}
