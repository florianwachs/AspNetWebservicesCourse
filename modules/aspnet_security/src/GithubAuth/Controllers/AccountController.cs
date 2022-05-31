using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("github/login")]
    public IActionResult Login(string? redirectUri = "/")
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        };

        return Challenge(authenticationProperties, GitHubAuthenticationDefaults.AuthenticationScheme);
    }
}
