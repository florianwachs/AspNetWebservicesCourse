using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TechConf.OData.Data;
using TechConf.OData.Models;

namespace TechConf.OData.Controllers;

public class SessionsController : ODataController
{
    private readonly AppDbContext _db;

    public SessionsController(AppDbContext db) => _db = db;

    [EnableQuery]
    public IQueryable<Session> Get()
        => _db.Sessions;

    [EnableQuery]
    public async Task<IActionResult> Get([FromRoute] Guid key)
    {
        var session = await _db.Sessions.FindAsync(key);
        return session is null ? NotFound() : Ok(session);
    }
}
