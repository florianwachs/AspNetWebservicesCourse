using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;
using TechConf.McpServer.Models;

namespace TechConf.McpServer.Tools;

[McpServerToolType]
public static class RegisterAttendeeTool
{
    [McpServerTool(Name = "register_attendee"), Description("Register an attendee for a TechConf event. Checks capacity limits.")]
    public static async Task<string> ExecuteAsync(
        AppDbContext db,
        [Description("The event ID to register for")] Guid eventId,
        [Description("Attendee's full name")] string name,
        [Description("Attendee's email address")] string email,
        CancellationToken ct = default)
    {
        var ev = await db.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == eventId, ct);

        if (ev is null)
            return $"❌ Event with ID {eventId} not found";

        if (ev.Registrations.Count >= ev.MaxAttendees)
            return $"❌ Event '{ev.Title}' is full ({ev.MaxAttendees} max)";

        if (ev.Registrations.Any(r => r.Attendee?.Email == email))
            return $"⚠️ {email} is already registered for '{ev.Title}'";

        var attendee = await db.Attendees.FirstOrDefaultAsync(a => a.Email == email, ct)
            ?? new Attendee { Id = Guid.NewGuid(), Name = name, Email = email };

        if (attendee.Id == Guid.Empty)
        {
            attendee.Id = Guid.NewGuid();
            db.Attendees.Add(attendee);
        }

        db.Registrations.Add(new Registration
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            AttendeeId = attendee.Id,
            RegisteredAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
        return $"✅ {name} registered for '{ev.Title}'";
    }
}
