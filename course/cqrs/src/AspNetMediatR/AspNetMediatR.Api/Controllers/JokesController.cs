using AspNetMediatR.Api.Domain.Jokes.Models;
using AspNetMediatR.Api.Domain.Jokes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetMediatR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {

        public JokesController(IMediator mediator)
        {
            Mediator = mediator;
        }

        public IMediator Mediator { get; }

        [HttpGet]
        public async Task<IActionResult> GetJokes() => Ok(await Mediator.Send(new JokesQuery(), CancellationToken.None));
    }
}
