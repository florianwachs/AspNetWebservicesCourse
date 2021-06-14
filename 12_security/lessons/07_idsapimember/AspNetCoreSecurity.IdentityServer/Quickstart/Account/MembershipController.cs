using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StsServerIdentity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace StsServerIdentity.Quickstart.Account
{
    [ApiController]
    [Route("api/v1/memberships")]
    [Authorize(LocalApi.PolicyName)]
    public class MembershipController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = AppPolicies.CanCreateUsers)]
        public async Task<ActionResult> GetTest()
        {
            // Hier z.B. user anlegen wie in SeedData.cs->CreateUser
            return Ok("Hello World");
        }
    }
}
