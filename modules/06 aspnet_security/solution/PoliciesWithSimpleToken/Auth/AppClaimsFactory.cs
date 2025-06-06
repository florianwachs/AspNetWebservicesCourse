using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PoliciesWithSimpleToken.Domain;

namespace PoliciesWithSimpleToken.Auth;

public class AppClaimsFactory : IUserClaimsPrincipalFactory<AppUser>
{
    public Task<ClaimsPrincipal> CreateAsync(AppUser user)
    {
        var claims = new List<Claim>() {
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Authentication, "true"),
            new Claim(AuthConstants.ClaimTypes.Age, user.Age.ToString()),
        };

        if (user.IsAdmin)
        {
            claims.Add(new(AuthConstants.ClaimTypes.IsAdmin, ""));
        }

        if (user.IsContentManager)
        {
            claims.Add(new(AuthConstants.ClaimTypes.IsContentManager, ""));
        }
        
        var claimsIdentity = new ClaimsIdentity(claims, "Bearer");

        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(claimsPrincipal);
    }
}