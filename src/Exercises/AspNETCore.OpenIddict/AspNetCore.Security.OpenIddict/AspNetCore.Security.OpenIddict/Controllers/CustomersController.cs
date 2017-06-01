using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Security.OpenIddict.Infrastructure;
using AspNetCore.Security.OpenIddict.Models;
using System.Threading;

namespace AspNetCore.Security.OpenIddict.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private static int id = 4;

        // Nur für Übungszwecke, Dictionary ist nicht Threadsafe!
        private static readonly Dictionary<string, Customer> people = new Dictionary<string, Customer>{
            {"1", new Customer{Id="1", Name="Jason Bourne", Age="30"}},
            {"2", new Customer{Id="2", Name="Captain America", Age="80"}},
            {"3", new Customer{Id="3", Name="Tony Stark", Age="40"}},
        };

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return people.Values;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (people.TryGetValue(id, out Customer c))
                return Ok(c);

            return NotFound();
        }

        [HttpPost]
        public IActionResult InsertCustomer(Customer p)
        {
            p.Id = GetUniqueId();
            people.Add(p.Id, p);
            return Ok(p);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(string id, Customer p)
        {
            if (!people.ContainsKey(id))
            {
                return NotFound();
            }

            p.Id = id;
            people[id] = p;
            return Ok(p);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            if (!people.ContainsKey(id))
            {
                return NotFound();
            }

            people.Remove(id);
            return Ok();
        }

        private string GetUniqueId()
        {
            return (Interlocked.Increment(ref id)).ToString();
        }

    }
}