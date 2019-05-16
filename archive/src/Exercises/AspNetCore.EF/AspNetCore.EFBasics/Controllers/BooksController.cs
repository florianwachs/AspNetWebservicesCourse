using AspNetCore.EFBasics.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCore.EFBasics.Models;

namespace AspNetCore.EFBasics.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        public BookDbContext BookDbContext { get; set; }

        public BooksController(BookDbContext bookDbContext)
        {
            BookDbContext = bookDbContext;
        }
        // GET api/books
        [HttpGet("")]
        public IEnumerable<Book> GetBooks()
        {
            return BookDbContext.Books;
        }

        // GET api/books/1
        [HttpGet("{id:int}", Name = "BookById")]
        public IActionResult GetBookById(int id)
        {
            var book = BookDbContext.Books.Find(id);
            return book != null ? (IActionResult)Ok(book) : NotFound();
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookDbContext.Books.Add(book);
            BookDbContext.SaveChanges();

            return Created(Url.Link("BookById", new { Id = book.Id }), book);
        }

        private string GetErrorMessage()
        {
            return string.Join(";", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateBook(int id, [FromBody]Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existing = BookDbContext.Books.Where(b => b.Id == id).FirstOrDefault();

            if (existing == null)
            {
                return NotFound();
            }

            BookDbContext.Books.Update(book);

            BookDbContext.SaveChanges();
            return Ok(book);
        }        

    }
}
