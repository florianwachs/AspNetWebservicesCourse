using System.Collections.Generic;
using System.Web.Http;
using WebAPI.IoC.Autofac.Models;
using WebAPI.IoC.Autofac.UnitOfWork;

namespace WebAPI.IoC.Autofac.Controllers
{
    [RoutePrefix("api/[Controller]")]
    public class PersonsController : ApiController
    {

        private readonly IRepository<Person> _personRepository;

        public PersonsController(IRepository<Person> personsRepository)
        {
            _personRepository = personsRepository;
        }

        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return _personRepository.GetAll();
        }

        [HttpGet]
        public Person Get(int id)
        {
            return _personRepository.Find(id);
        }
    }
}
