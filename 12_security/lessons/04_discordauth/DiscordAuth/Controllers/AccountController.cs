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
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("token")]
        [Authorize]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var authenticationInfo = await HttpContext.AuthenticateAsync();
            return Ok(accessToken);
        }


        [HttpGet("discord/login")]
        public IActionResult Login(string redirectUri = "/")
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
            };

            return Challenge(authenticationProperties, "Discord");
        }


    }
}
