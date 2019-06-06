using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SwaggerLesson.Models;

namespace SwaggerLesson.Api.Books
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        /// <summary>
        /// Returns all books
        /// </summary>
        /// <returns>all books</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            return Ok(await _bookRepository.GetAll());
        }

        /// <summary>
        /// Returns a single book
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     GET /books/1
        ///
        /// </remarks>
        /// <param name="id">id of the book</param>
        /// <returns>a book</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(string id)
        {
            var book = await _bookRepository.GetById(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="book">data for the book to create</param>
        /// <returns>the newly created book</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> CreateNew([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookRepository.Add(book);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Complete update of a book
        /// </summary>
        /// <param name="id">id of the book to update</param>
        /// <param name="book">data of the book</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Update(string id, [FromBody] Book book)
        {
            var exists = _bookRepository.GetById(id) != null;

            if (!exists)
            {
                return BadRequest();
            }

            var result = await _bookRepository.Update(book);
            return Ok(result);
        }

        /// <summary>
        /// removes a book
        /// </summary>
        /// <param name="id">id of the book to remove</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _bookRepository.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Updates a book partially by the provided properties
        /// </summary>
        /// <param name="id">id of the book to update</param>
        /// <param name="doc">operations to perform</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<Book>> PartialUpdate(string id, [FromBody] JsonPatchDocument<Book> doc)
        {
            var existing = await _bookRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            doc.ApplyTo(existing);
            var result = await _bookRepository.Update(existing);

            return Ok(result);
        }
    }
}