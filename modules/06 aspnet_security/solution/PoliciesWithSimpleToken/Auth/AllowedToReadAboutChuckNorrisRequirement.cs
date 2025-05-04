using Microsoft.AspNetCore.Authorization;

namespace PoliciesWithSimpleToken.Auth;

public class AllowedToReadAboutChuckNorrisRequirement : IAuthorizationRequirement
{
}

public class AllowedToReadAboutChuckNorrisRequirementHandler : AuthorizationHandler<AllowedToReadAboutChuckNorrisRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedToReadAboutChuckNorrisRequirement requirement)
    {
        var over30 = context.User.Claims.Any(c => c.Type == AuthConstants.ClaimTypes.Age && int.Parse(c.Value) > 30);
        var isAdmin = context.User.Claims.Any(c => c.Type == AuthConstants.ClaimTypes.IsAdmin);
        if (over30 || isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

