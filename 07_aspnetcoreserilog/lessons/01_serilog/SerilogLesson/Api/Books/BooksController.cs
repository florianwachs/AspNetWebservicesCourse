using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SerilogLesson.Models;

namespace SerilogLesson.Api.Books
{
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger _logger;


        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _bookRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _bookRepository.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("No book found with id {bookId}", id);
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookRepository.Add(book);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Book book)
        {
            var exists = _bookRepository.GetById(id) != null;

            if (!exists)
            {
                _logger.LogWarning("No book found with id {bookId}", id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Book with Id {bookId} can´t be updated because of Validation Errors {@errors}", id,
                    new SerializableError(ModelState));
            }

            var result = await _bookRepository.Update(book);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _bookRepository.Delete(id);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(string id, [FromBody] JsonPatchDocument<Book> doc)
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