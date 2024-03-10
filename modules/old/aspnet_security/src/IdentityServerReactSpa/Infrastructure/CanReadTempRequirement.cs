using Microsoft.AspNetCore.Authorization;
using ReactAppWithAuth1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactAppWithAuth1.Infrastructure
{
    public class CanReadTempRequirement : IAuthorizationRequirement
    {
    }

    public class CanReadTempRequirementHandler : AuthorizationHandler<CanReadTempRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadTempRequirement requirement)
        {
            var user = context.User;
            var canReadTemp = user.HasClaim(c => c.Type == IsPremiumUserClaim.Type || c.Type == CanAddWeatherClaim.Type);
            if (canReadTemp)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
