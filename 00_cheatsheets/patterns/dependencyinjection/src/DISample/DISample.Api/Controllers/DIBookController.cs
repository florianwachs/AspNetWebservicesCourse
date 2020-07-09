using DISample.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISample.Api.Controllers
{
    [ApiController]
    [Route("api/dibooks")]
    public class DIBookController : ControllerBase
    {
        public DIBookController(IBookRepository bookRepository, ITimeService timeService)
        {
            BookRepository = bookRepository;
            TimeService = timeService;
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
