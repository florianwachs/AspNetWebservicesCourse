using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
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

        [HttpGet("random")]
        public async Task<ActionResult<Joke>> GetRandomJoke()
        {
            return Ok(await _jokeProvider.GetRandomJoke());
        }
    }
}
