using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Server.Models;

namespace WebAPI.Server.Controllers
{
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
        public IEnumerable<Person> Get()
        {
            return personen.Values;
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetById(string id)
        {
            Person p;
            if (personen.TryGetValue(id, out p))
            {
                return Ok(p);
            }

            return NotFound();
        }

        [Route("")]
        [ResponseType(typeof(Person))]
        [HttpPost]
        public IHttpActionResult InsertPerson(Person p)
        {
            p.Id = GetUniqueId();
            personen.Add(p.Id, p);
            return Ok(p);
        }

        [Route("{id}")]
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
    }
}
