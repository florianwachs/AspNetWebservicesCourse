using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore1.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        public BooksController(IBookRepository bookRepository, ITimeService timeService)
        {
            BookRepository = bookRepository;
            TimeService = timeService;
        }

        private IBookRepository BookRepository { get; }
        public ITimeService TimeService { get; }

        // GET api/books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return Ok(BookRepository.GetAll());
        }

        // GET api/books/1
        [HttpGet("{id}")]
        public ActionResult<Book> GetById(int id)
        {
            var book = BookRepository.GetById(id);
            if (book == null)
            {
                return NotFound("Kein Buch zu dieser ID");
            }

            return Ok(book);
        }

        // POST api/books           
        [HttpPost]
        public IActionResult CreateBook(Book book)
        {
            if (book == null)
            {
                return BadRequest("so nicht mein freund!!");
            }
            // TODO: Never TRUST the Client, Validation

            BookRepository.Add(book);

            return CreatedAtAction("GetBookById", new { id = book.Id }, book);
            //return CreatedAtAction(nameof(GetBookHaltso), new { id = book.Id }, book);
        }
    }
}
