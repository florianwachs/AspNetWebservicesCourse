using DISample.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISample.Api.Controllers
{
    [ApiController]
    [Route("api/nodibooks")]
    public class NoDIBookController : ControllerBase
    {
        public NoDIBookController()
        {
            BookRepository = new DummyBookRepository();
            TimeService = new DefaultTimeService();
        }

        public IBookRepository BookRepository { get; }
        public ITimeService TimeService { get; }

        [HttpGet]
        public IEnumerable<Book> GetAllBooks()
        {
            var currentTime = TimeService.Now;
            return BookRepository.All();
        }

    }
}
