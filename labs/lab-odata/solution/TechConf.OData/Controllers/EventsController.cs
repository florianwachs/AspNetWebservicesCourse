using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TechConf.OData.Data;
using TechConf.OData.Models;

namespace TechConf.OData.Controllers;

public class EventsController : ODataController
{
    private readonly AppDbContext _db;

    public EventsController(AppDbContext db) => _db = db;

    [EnableQuery(PageSize = 50)]
    public IQueryable<Event> Get()
        => _db.Events;

    [EnableQuery]
    public async Task<IActionResult> Get([FromRoute] Guid key)
    {
        var ev = await _db.Events.FindAsync(key);
        return ev is null ? NotFound() : Ok(ev);
    }

    public async Task<IActionResult> Post([FromBody] Event ev)
    {
        if (ev.Id == Guid.Empty)
            ev.Id = Guid.NewGuid();

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return Created(ev);
    }

    public async Task<IActionResult> Patch([FromRoute] Guid key,
        [FromBody] Delta<Event> delta)
    {
        var ev = await _db.Events.FindAsync(key);
        if (ev is null) return NotFound();

        delta.Patch(ev);
        await _db.SaveChangesAsync();
        return Updated(ev);
    }

    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var ev = await _db.Events.FindAsync(key);
        if (ev is null) return NotFound();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> AvailableSeats([FromRoute] Guid key)
    {
        var ev = await _db.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == key);
        if (ev is null) return NotFound();

        return Ok(ev.MaxAttendees - ev.Registrations.Count);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel([FromRoute] Guid key)
    {
        var ev = await _db.Events.FindAsync(key);
        if (ev is null) return NotFound();

        ev.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return Ok(ev);
    }
}
