using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecurity.Api.Health
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthInfosController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAuthInfosForCurrentUser()
        {
            return Ok((User.Claims ?? Array.Empty<Claim>()).Select(c => new { c.Type, c.Value }));
        }
    }
}
