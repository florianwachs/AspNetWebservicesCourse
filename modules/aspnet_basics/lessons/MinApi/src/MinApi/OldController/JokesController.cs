using ChuckNorrisService.Models;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _jokeProvider.Delete(id);
            return Ok();
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomJoke()
        {
            return Ok(await _jokeProvider.GetRandomJoke());
        }
    }
}
