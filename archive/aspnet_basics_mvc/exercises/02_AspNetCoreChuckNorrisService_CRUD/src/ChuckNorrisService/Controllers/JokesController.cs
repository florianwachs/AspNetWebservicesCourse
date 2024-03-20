using ChuckNorrisService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ChuckNorrisService.Controllers
{
    [Route("api/[controller]")]
    public class JokesController : ControllerBase
    {
        private readonly IJokeRepository _jokeRepository;

        public JokesController(IJokeRepository jokeRepository)
        {
            _jokeRepository = jokeRepository;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomJoke()
        {
            return Ok(await _jokeRepository.GetRandomJoke());
        }
    }
}
