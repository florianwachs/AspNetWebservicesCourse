using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreSwagger.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSwagger.Api.Jokes
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class JokesController : ControllerBase
    {
        private readonly IJokeRepository _jokeRepository;

        public JokesController(IJokeRepository jokeRepository)
        {
            _jokeRepository = jokeRepository;
        }

        /// <summary>
        /// Returns all jokes
        /// </summary>
        /// <returns>all jokes</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Joke>>> GetAll()
        {
            return Ok(await _jokeRepository.GetAll());
        }

        /// <summary>
        /// Returns a single joke
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     GET /jokes/1
        ///
        /// </remarks>
        /// <param name="id">id of the joke</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Joke>> GetById(string id)
        {
            var joke = await _jokeRepository.GetById(id);
            if (joke == null)
            {
                return NotFound();
            }

            return Ok(joke);
        }

        /// <summary>
        /// Creates a new joke
        /// </summary>
        /// <param name="joke">data for the joke to create</param>
        /// <returns>the newly created joke</returns>
        [HttpPost]
        public async Task<ActionResult<Joke>> CreateNew([FromBody] Joke joke)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jokeRepository.Add(joke);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        /// <summary>
        /// Complete update of a joke
        /// </summary>
        /// <param name="id">id of the joke to update</param>
        /// <param name="joke">data of the joke</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Joke>> Update(string id, [FromBody] Joke joke)
        {
            var exists = _jokeRepository.GetById(id) != null;

            if (!exists)
            {
                return BadRequest();
            }

            var result = await _jokeRepository.Update(joke);
            return Ok(result);
        }

        /// <summary>
        /// removes a joke
        /// </summary>
        /// <param name="id">id of the joke to remove</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _jokeRepository.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Updates a joke partially by the provided properties
        /// </summary>
        /// <param name="id">id of the joke to update</param>
        /// <param name="doc">operations to perform</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<Joke>> PartialUpdate(string id, [FromBody] JsonPatchDocument<Joke> doc)
        {
            var existing = await _jokeRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            doc.ApplyTo(existing);
            var result = await _jokeRepository.Update(existing);

            return Ok(result);
        }

        /// <summary>
        /// Returns a random joke
        /// </summary>
        /// <returns>a joke</returns>
        [HttpGet("random")]
        public async Task<ActionResult<Joke>> GetRandomJoke()
        {
            return Ok(await _jokeRepository.GetRandomJoke());
        }
    }
}