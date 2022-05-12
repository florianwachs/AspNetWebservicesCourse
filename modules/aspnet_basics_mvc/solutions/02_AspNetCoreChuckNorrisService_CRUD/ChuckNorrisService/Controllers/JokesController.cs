using ChuckNorrisService.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ChuckNorrisService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {
        private readonly IJokeRepository _jokeProvider;

        public JokesController(IJokeRepository jokeProvider)
        {
            _jokeProvider = jokeProvider;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            Joke joke = await _jokeProvider.GetById(id);
            if (joke == null)
            {
                return NotFound();
            }

            return Ok(joke);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew([FromBody] Joke joke)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Joke result = await _jokeProvider.Add(joke);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Joke joke)
        {
            bool exists = _jokeProvider.GetById(id) != null;

            if (!exists)
            {
                return BadRequest();
            }

            Joke result = await _jokeProvider.Update(joke);
            return CreatedAtAction(nameof(GetById), new { id = joke.Id }, joke);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _jokeProvider.Delete(id);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdate(string id, [FromBody] JsonPatchDocument<Joke> doc)
        {
            Joke existing = await _jokeProvider.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            doc.ApplyTo(existing);
            Joke result = await _jokeProvider.Update(existing);

            return Ok(result);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomJoke()
        {
            return Ok(await _jokeProvider.GetRandomJoke());
        }
    }
}
