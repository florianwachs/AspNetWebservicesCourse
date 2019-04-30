using BookCatalog.API.Infrastructure;
using BookCatalog.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;

namespace BookCatalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class BooksController : Controller
    {
        private readonly BookDbContext _bookContext;
        private readonly PriceEngine _priceEngine;
        private readonly RatingEngine _ratingEngine;

        public BooksController(BookDbContext bookContext, PriceEngine priceEngine, RatingEngine ratingEngine)
        {
            _bookContext = bookContext;
            _priceEngine = priceEngine;
            _ratingEngine = ratingEngine;
        }

        // GET api/books
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        public async Task<List<BookDto>> GetBooks()
        {
            var books = await _bookContext.Books.ToListAsync();
            var bookDtos = books.Select(GetBookDto).ToList();      

            return bookDtos;
        }

        private BookDto GetBookDto(Book book)
        {
            var dto = new BookDto
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                ReleaseDate = book.ReleaseDate,
                Price = _priceEngine.GetPriceForBook(book.Id)?.Amount ?? 0m,
                Rating = _ratingEngine.GetRatingForBook(book.Id),
            };
            return dto;
        }

        // GET api/books/1
        [HttpGet("{id:int}", Name = "BookById")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookContext.Books.FindAsync(id);
            return book != null ? (IActionResult)Ok(GetBookDto(book)) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Book), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book = (await _bookContext.Books.AddAsync(book)).Entity;
            return Created(Url.Link("BookById", new { Id = book.Id }), book);
        }

        private string GetErrorMessage()
        {
            return string.Join(";", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Book), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Book), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody]Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existing = await _bookContext.Books.FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            book = _bookContext.Books.Update(book).Entity;
            return Ok(book);
        }
    }

}
