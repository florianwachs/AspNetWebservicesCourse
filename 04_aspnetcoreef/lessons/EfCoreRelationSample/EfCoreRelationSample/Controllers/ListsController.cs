using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRelationSample.DataAccess;
using EfCoreRelationSample.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreRelationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly DemoDbContext _demoDbContext;

        // Das Datenmodell sollte nie direkt über die API ausgegeben werden
        // Immer ViewModels verwenden. Hier wird das nur zu Demozwecken gemacht
        // um das Beispiel einfach zu halten.
        public ListsController(DemoDbContext demoDbContext)
        {
            _demoDbContext = demoDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> Get()
        {
            var lists = await _demoDbContext.Lists.ToListAsync();
            return lists;
        }

        [HttpGet("{id}/sharedwith")]
        public async Task<ActionResult<IEnumerable<User>>> GetSharedUsers(string id)
        {
            var list = await _demoDbContext.Lists.Include(l => l.SharedWith).ThenInclude(u => u.User).Where(l => l.Id == id).FirstOrDefaultAsync();

            if (list == null)
                return NotFound();

            return Ok(list.SharedWith.Select(share => share.User));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
