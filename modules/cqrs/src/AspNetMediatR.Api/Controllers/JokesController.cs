using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;
using AspNetMediatR.Api.Queries;

namespace AspNetMediatR.Api.Controllers
{
    [Route("api/v1/[controller]")]
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
