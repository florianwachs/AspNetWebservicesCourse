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

        private static List<Book> _books = new List<Book>() {
            new Book {Id=1, Name = "Test1", Isbn = "ISBN1" } ,
            new Book {Id=2, Name = "Flo wann ist endlich schluss?", Isbn = "ISBN2" }
        };

        // GET api/books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return Ok(_books);
        }

        // GET api/books/1
        [HttpGet("{id}")]
        public ActionResult<Book> GetBookHaltso(int id)
        {
            var book = _books.Where(book => book.Id == id).FirstOrDefault();
            if (book == null)
            {
                return NotFound("Kein Buch zu dieser ID");
            }

            return Ok(book);
        }

        // GET api/books/1/reviews
        [HttpGet("{id}/reviews")]
        public ActionResult<IEnumerable<string>> GetBookReviewsById(int id)
        {
            return Ok(new[] { "So Super!! Kaufen" });
        }

        // GET api/books/1/reviews/1
        [HttpGet("{bookId}/reviews/{reviewId}")]
        public ActionResult<IEnumerable<string>> GetBookReviewById(int bookId, int reviewId)
        {
            return Ok("So Super!! Kaufen");
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

            _books.Add(book);

            return CreatedAtAction("GetBookById", new { id = book.Id }, book);
            //return CreatedAtAction(nameof(GetBookHaltso), new { id = book.Id }, book);
        }

        // PUT api/books/1      
        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, Book book)
        {
            throw new NotImplementedException();
        }

    }
}
