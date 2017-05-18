using AspNetCore.EFRepository.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCore.EFRepository.Models;
using AspNetCore.EFRepository.Repositories;

namespace AspNetCore.EFRepository.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private IBookRepository BookRepository { get; set; }
        public BooksController(IBookRepository bookRepository)
        {
            BookRepository = bookRepository;
        }
        // GET api/books
        [HttpGet("")]
        public IEnumerable<Book> GetBooks()
        {
            return BookRepository.GetAll();
        }

        // GET api/books/1
        [HttpGet("{id:int}", Name = "BookById")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await BookRepository.GetById(id);
            return book != null ? (IActionResult)Ok(book) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book = await BookRepository.Add(book);
            return Created(Url.Link("BookById", new { Id = book.Id }), book);
        }

        private string GetErrorMessage()
        {
            return string.Join(";", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody]Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existing = await BookRepository.GetById(id);

            if (existing == null)
            {
                return NotFound();
            }

            book = await BookRepository.Update(id, book);
            return Ok(book);
        }

    }
}
