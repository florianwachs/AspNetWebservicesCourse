using System.Security.Claims;

namespace TechConf.Api.Infrastructure.Auth;

public interface ICurrentUserAccessor
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    string? Email { get; }
    ClaimsPrincipal Principal { get; }
    bool IsInRole(string role);
    bool HasClaim(string type, string value);
}

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public ClaimsPrincipal Principal =>
        httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;

    public string? UserId => Principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email => Principal.FindFirstValue(ClaimTypes.Email);

    public bool IsInRole(string role) => Principal.IsInRole(role);

    public bool HasClaim(string type, string value) => Principal.HasClaim(type, value);
}
