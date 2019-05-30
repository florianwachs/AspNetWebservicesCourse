using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuckNorrisService.Controllers
{
    [Route("api/[controller]")]
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
            var joke = await _jokeProvider.GetById(id);
            if (joke == null)
            {
                return NotFound();
            }

            return Ok(joke);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew([FromBody]Joke joke)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jokeProvider.Add(joke);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]Joke joke)
        {
            var exists = _jokeProvider.GetById(id) != null;

            if (!exists)
            {
                return BadRequest();
            }

            var result = await _jokeProvider.Update(joke);
            return Ok(result);
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
            var existing = await _jokeProvider.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            doc.ApplyTo(existing);
            var result = await _jokeProvider.Update(existing);

            return Ok(result);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomJoke()
        {
            return Ok(await _jokeProvider.GetRandomJoke());
        }
    }
}
