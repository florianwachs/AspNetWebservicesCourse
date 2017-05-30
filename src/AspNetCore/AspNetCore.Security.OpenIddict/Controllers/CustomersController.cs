using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Security.OpenIddict.Infrastructure;
using AspNetCore.Security.OpenIddict.Models;

namespace AspNetCore.Security.OpenIddict.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private static int id = 4;

        private static readonly Dictionary<string, Customer> people = new Dictionary<string, Customer>{
            {"1", new Customer{Id="1", Name="Jason Bourne", Age="30"}},
            {"2", new Customer{Id="2", Name="Captain America", Age="80"}},
            {"3", new Customer{Id="3", Name="Tony Stark", Age="40"}},
        };

        public CustomersController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public Task<IEnumerable<Customer>> Get()
        {
            return GetSecuredData();
        }

        private async Task<IEnumerable<Customer>> GetSecuredData()
        {
            var includeAge = await FullfilesPolicy(AppPolicies.CanReadCustomerAge);
            return people.Values
                .Select(p =>
                    new Customer
                    {
                        Name = p.Name,
                        Id = p.Id,
                        Age = includeAge ? p.Age : "TOP SECRET"
                    }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            Customer p;
            var map = (await GetSecuredData()).ToDictionary(x => x.Id);
            if (map.TryGetValue(id, out p))
            {
                return Ok(p);
            }

            return NotFound();
        }

        [Authorize(Policy = AppPolicies.CanCreateCustomer)]
        [HttpPost]
        public IActionResult InsertCustomer(Customer p)
        {
            p.Id = GetUniqueId();
            people.Add(p.Id, p);
            return Ok(p);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AppPolicies.CanUpdateCustomer)]
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
        [Authorize(Policy = AppPolicies.CanDeleteCustomer)]
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
            return (id++).ToString();
        }
        private Task<bool> FullfilesPolicy(string policy)
        {
            return _authorizationService.AuthorizeAsync(User, policy);
        }
    }
}