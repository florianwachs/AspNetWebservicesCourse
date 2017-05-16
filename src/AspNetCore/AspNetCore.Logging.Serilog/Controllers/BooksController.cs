using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using AspNetCore.Logging.Serilog.Repositories;
using AspNetCore.Logging.Serilog.Infrastructure;
using AspNetCore.Logging.Serilog.Models;

namespace AspNetCore.Logging.Serilog.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private IBookRepository BookRepository { get; set; }
        private ITimeService TimeService { get; set; }
        private ILogger Logger { get; set; }

        public BooksController(ILogger<BooksController> logger, IBookRepository bookRepository, ITimeService timeService)
        {
            Logger = logger;
            BookRepository = bookRepository;
            TimeService = timeService;
        }
        // GET api/books
        [HttpGet("")]
        public IEnumerable<Book> GetBooks()
        {
            return BookRepository.GetAll();
        }

        // GET api/books/1
        [HttpGet("{id:int}", Name = "BookById")]
        public IActionResult GetBookById(int id)
        {
            var book = BookRepository.GetById(id);
            if (book != null)
            {
                Logger.LogInformation("Book with Id {Id} was requested and found.", id);
                return Ok(book);
            }
            else
            {
                Logger.LogWarning("No book was found by the provided Id {Id}.", id);
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                Logger.LogError("The supplied data for book creation is invalid ({@Book}) with Errors {Errors}.", book, GetErrorMessage());
                return BadRequest(ModelState);
            }

            var createdbook = BookRepository.Add(book);
            Logger.LogTrace("New book added to Repository ({@Book}).", book);

            var bookLocationUrl = Url.Link("BookById", new { Id = createdbook.Id });
            Logger.LogDebug("Location of new Book with id {Id} is {LocationUrl}.", book.Id, bookLocationUrl);
            return Created(bookLocationUrl, createdbook);
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
                Logger.LogError("Unable to update book ({@Book}) with id {Id} because {Errors}.", book, id, GetErrorMessage());
                return BadRequest();
            }

            var existing = BookRepository.GetById(id);

            if (existing == null)
            {
                Logger.LogError("Unable to update book with id {Id} because book not found", id);
                return NotFound();
            }

            book = BookRepository.Update(id, book);
            return Ok(book);
        }

    }
}
