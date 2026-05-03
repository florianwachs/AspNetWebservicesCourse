using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TechConf.OData.Data;
using TechConf.OData.Models;

namespace TechConf.OData.Controllers;

// TODO: Task 3 — Implement SessionsController
//
// This controller should:
// 1. Inherit from ODataController
// 2. Have a constructor that injects AppDbContext
// 3. Implement GET /odata/Sessions with [EnableQuery]
//    - Return IQueryable<Session> so $expand=Speaker works
// 4. Implement GET /odata/Sessions({key}) with [EnableQuery]
//    - Find by Guid key, return NotFound() or Ok(session)

public class SessionsController : ODataController
{
    private readonly AppDbContext _db;

    public SessionsController(AppDbContext db) => _db = db;

    public IQueryable<Session> Get()
    {
        throw new NotImplementedException("Return _db.Sessions as IQueryable");
    }

    [EnableQuery]
    public async Task<IActionResult> Get([FromRoute] Guid key)
    {
        throw new NotImplementedException("Find session by key");
    }
}
