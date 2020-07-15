using DiscordAuth.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAuth.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscordInfoController : ControllerBase
    {
        public DiscordApiClient DiscordApi { get; }
        public DiscordInfoController(DiscordApiClient discordApi)
        {
            DiscordApi = discordApi;
        }        

        [HttpGet("me")]
        public async Task<IActionResult> GetInfoAboutMe()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var info = await DiscordApi.GetUserInfo(token);

            return Ok(info);
        }
    }
}
