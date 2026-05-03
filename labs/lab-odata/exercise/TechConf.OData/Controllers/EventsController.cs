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

    // TODO: Task 2 — Implement GET /odata/Events
    // Return IQueryable<Event> with [EnableQuery(PageSize = 50)]
    // This allows OData to apply $filter, $select, $orderby, $top, $skip, $expand
    [EnableQuery]
    public IQueryable<Event> Get()
    {
        throw new NotImplementedException("Return _db.Events as IQueryable");
    }

    // TODO: Task 2 — Implement GET /odata/Events({key})
    // Find event by Guid key, return NotFound() or Ok(event)
    // Add [EnableQuery] to allow $select and $expand on single entity
    [EnableQuery]
    public async Task<IActionResult> Get([FromRoute] Guid key)
    {
        throw new NotImplementedException("Find event by key");
    }

    // TODO: Task 2 — Implement POST /odata/Events
    // Add the event to db, return Created(ev)
    public async Task<IActionResult> Post([FromBody] Event ev)
    {
        throw new NotImplementedException("Create a new event");
    }

    // TODO: Task 2 — Implement PATCH /odata/Events({key})
    // Use Delta<Event> to apply partial updates
    // Find the event, call delta.Patch(ev), save, return Updated(ev)
    public async Task<IActionResult> Patch([FromRoute] Guid key,
        [FromBody] Delta<Event> delta)
    {
        throw new NotImplementedException("Apply partial update with Delta<T>");
    }

    // TODO: Task 2 — Implement DELETE /odata/Events({key})
    // Find the event, remove it, return NoContent()
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        throw new NotImplementedException("Delete an event");
    }

    // TODO: Task 4 — Implement AvailableSeats function
    // GET /odata/Events({key})/AvailableSeats
    // Load event with Registrations, return MaxAttendees - Registrations.Count
    [HttpGet]
    public async Task<IActionResult> AvailableSeats([FromRoute] Guid key)
    {
        throw new NotImplementedException("Return available seats count");
    }

    // TODO: Task 5 — Implement Cancel action
    // POST /odata/Events({key})/Cancel
    // Set event Status to "Cancelled", save, return Ok(ev)
    [HttpPost]
    public async Task<IActionResult> Cancel([FromRoute] Guid key)
    {
        throw new NotImplementedException("Cancel an event");
    }
}
