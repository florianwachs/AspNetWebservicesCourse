using AspNetCoreHateoasWithLinks.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreHateoasWithLinks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetUserById")]
        public ActionResult<UserDto> GetUserById(string id)
        {
            return Ok(new UserDto { Id = id, Name = "Chuck Norris" });
        }
    }
}
