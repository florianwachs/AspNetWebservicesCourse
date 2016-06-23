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
            PersonRepository = new EFPersonRepository();
        }

        [Route("")]
        public IEnumerable<Person> Get()
        {
            return PersonRepository.GetAll();
        }

        [Route("{id:int}")]
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetById(int id)
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
            if (p == null || !ModelState.IsValid)
                return BadRequest();

            var addedPerson = PersonRepository.Add(p);
            return Ok(addedPerson);
        }

        [Route("{id:int}")]
        [ResponseType(typeof(Person))]
        [HttpPut]
        public IHttpActionResult UpdatePerson(int id, Person p)
        {
            if (p == null || !ModelState.IsValid)
                return BadRequest();

            PersonRepository.Update(id, p);
            return Ok(p);
        }

        [Route("{id:int}")]
        [ResponseType(typeof(Person))]
        [HttpDelete]
        public IHttpActionResult DeletePerson(int id)
        {
            var success = PersonRepository.Delete(id);

            if (success)
                return Ok();
            else
                return NotFound();
        }
    }
}
