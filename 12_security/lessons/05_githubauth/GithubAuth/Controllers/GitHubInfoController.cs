using GithubAuth.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubAuth.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GitHubInfoController : ControllerBase
    {
        public GitHubInfoController(GithubApiClient githubApi)
        {
            GithubApi = githubApi;
        }

        public GithubApiClient GithubApi { get; }

        [HttpGet("me")]
        public async Task<IActionResult> GetInfoAboutMe()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var info = await GithubApi.GetUserInfo(token);

            return Ok(info);
        }
    }
}
