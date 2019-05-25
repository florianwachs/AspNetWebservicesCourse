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
        private readonly FileSystemJokeProvider _jokesProvider;

        public JokesController()
        {
           _jokesProvider = new FileSystemJokeProvider();
        }

        [HttpGet("random")]
        public async Task<ActionResult<Joke>> GetRandomJoke()
        {
            return Ok(await _jokesProvider.GetRandomJokeAsync());
        }
    }
}
