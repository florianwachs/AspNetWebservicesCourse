using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Security.Infrastructure;
using WebAPI.Security.Models;

namespace WebAPI.Security.Controllers
{
    [Authorize]
    [RoutePrefix("api/personen")]
    public class PersonenController : ApiController
    {
        private static int id = 4;
        private static readonly Dictionary<string, Person> personen = new Dictionary<string, Person>{
            {"1", new Person{Id="1", Name="Jason Bourne", Age="30"}},
            {"2", new Person{Id="2", Name="Captain America", Age="80"}},
            {"3", new Person{Id="3", Name="Tony Stark", Age="40"}},
        };

        [Route("")]
        [AllowAnonymous]
        public IEnumerable<Person> Get()
        {
            return GetSecuredData();
        }

        private IEnumerable<Person> GetSecuredData()
        {
            return personen.Values
                .Select(p =>
                new Person
                {
                    Name = p.Name,
                    Id = p.Id,
                    Age = HasPermission(AppPermissions.CanReadPersonAge) ? p.Age : "TOP SECRET"
                }).ToList();
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetById(string id)
        {
            Person p;
            var map = GetSecuredData().ToDictionary(x => x.Id);
            if (map.TryGetValue(id, out p))
            {
                return Ok(p);
            }

            return NotFound();
        }

        [Route("")]
        [ClaimsAuthorize(Value = AppPermissions.CanCreatePerson)]
        [ResponseType(typeof(Person))]
        [HttpPost]
        public IHttpActionResult InsertPerson(Person p)
        {
            p.Id = GetUniqueId();
            personen.Add(p.Id, p);
            return Ok(p);
        }

        [Route("{id}")]
        [ClaimsAuthorize(Value = AppPermissions.CanUpdatePerson)]
        [ResponseType(typeof(Person))]
        [HttpPut]
        public IHttpActionResult UpdatePerson(string id, Person p)
        {
            if (!personen.ContainsKey(id))
            {
                return NotFound();
            }

            p.Id = id;
            personen[id] = p;
            return Ok(p);
        }

        [Route("{id}")]
        [ClaimsAuthorize(Value = AppPermissions.CanDeletePerson)]
        [ResponseType(typeof(Person))]
        [HttpDelete]
        public IHttpActionResult DeletePerson(string id)
        {
            if (!personen.ContainsKey(id))
            {
                return NotFound();
            }

            personen.Remove(id);
            return Ok();
        }

        private string GetUniqueId()
        {
            return (id++).ToString();
        }
        private bool HasPermission(string permission)
        {
            var principal = RequestContext.Principal as ClaimsPrincipal;
            return principal != null && principal.HasClaim(AppClaimTypes.Permission, permission);
        }
    }
}