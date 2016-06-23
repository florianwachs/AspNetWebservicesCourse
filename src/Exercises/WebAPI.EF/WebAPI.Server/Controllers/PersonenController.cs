using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Server.DataAccess;
using WebAPI.Server.Models;

namespace WebAPI.Server.Controllers
{
    [RoutePrefix("api/personen")]
    public class PersonenController : ApiController
    {
        private IPersonRepository PersonRepository { get; set; }
        public PersonenController()
        {
            PersonRepository = new InMemoryPersonService();
        }

        [Route("")]
        public IEnumerable<Person> Get()
        {
            return PersonRepository.GetAll();
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetById(string id)
        {
            var p = PersonRepository.GetById(id);
            if (p != null)
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
            var addedPerson = PersonRepository.Add(p);
            return Ok(addedPerson);
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        [HttpPut]
        public IHttpActionResult UpdatePerson(string id, Person p)
        {
            PersonRepository.Update(id, p);
            return Ok(p);
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        [HttpDelete]
        public IHttpActionResult DeletePerson(string id)
        {
            var success = PersonRepository.Delete(id);

            if (success)
                return Ok();
            else
                return NotFound();
        }
    }
}
