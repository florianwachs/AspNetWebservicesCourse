using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TechConf.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // TODO: Task 4 — Implement POST /register
        // Hint: Accept a RegisterRequest, create user with UserManager, assign "Attendee" role
        // Return validation errors if creation fails

        // TODO: Task 4 — Implement POST /login (cookie mode)
        // Hint: Accept a LoginRequest, use SignInManager.PasswordSignInAsync()
        // Return 401 on failure, 200 on success (cookie is set automatically)

        // ── JWT mode (alternative — uncomment to add /token endpoint) ───
        // This endpoint validates credentials and returns a JWT instead of setting a cookie.
        // group.MapPost("/token", async (LoginRequest request,
        //     UserManager<IdentityUser> userManager,
        //     IConfiguration config) =>
        // {
        //     var user = await userManager.FindByEmailAsync(request.Email);
        //     if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        //     {
        //         return Results.Problem("Invalid email or password.", statusCode: 401);
        //     }
        //
        //     var roles = await userManager.GetRolesAsync(user);
        //
        //     var claims = new List<Claim>
        //     {
        //         new(ClaimTypes.NameIdentifier, user.Id),
        //         new(ClaimTypes.Email, user.Email!),
        //     };
        //     claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        //
        //     var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
        //         System.Text.Encoding.UTF8.GetBytes(
        //             config["Jwt:Key"] ?? "super-secret-key-for-dev-only-min-32-chars!!"));
        //     var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
        //         key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        //
        //     var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        //         issuer: config["Jwt:Issuer"] ?? "https://techconf.dev",
        //         audience: config["Jwt:Audience"] ?? "techconf-api",
        //         claims: claims,
        //         expires: DateTime.UtcNow.AddHours(2),
        //         signingCredentials: creds);
        //
        //     var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        //     return Results.Ok(new TokenResponse(jwt, "Bearer", 7200));
        // });

        // TODO: Task 4 — Implement POST /logout (requires authorization)
        // Hint: Call SignInManager.SignOutAsync()

        // TODO: Task 4 — Implement GET /me (requires authorization)
        // Hint: Read claims from ClaimsPrincipal to return user id, email, and roles
    }
}

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password, bool RememberMe = false);

public record UserInfoResponse(string? Id, string? Email, List<string> Roles);

// Used by the JWT /token endpoint
public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
