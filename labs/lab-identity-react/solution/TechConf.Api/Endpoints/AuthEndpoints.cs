using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TechConf.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, UserManager<IdentityUser> userManager) =>
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Results.ValidationProblem(
                    result.Errors.GroupBy(e => e.Code).ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()));
            }

            // All self-registered users get the Attendee role
            await userManager.AddToRoleAsync(user, "Attendee");
            return Results.Ok(new { message = "Registration successful" });
        });

        // ── Cookie mode login (active) ──────────────────────────────────
        group.MapPost("/login", async (LoginRequest request, SignInManager<IdentityUser> signInManager) =>
        {
            var result = await signInManager.PasswordSignInAsync(
                request.Email, request.Password, isPersistent: request.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Results.Problem("Invalid email or password.", statusCode: 401);
            }

            return Results.Ok(new { message = "Login successful" });
        });

        // ── JWT mode login (uncomment to add /token endpoint) ───────────
        // This endpoint validates credentials and returns a JWT instead of setting a cookie.
        // Use this when the React frontend is configured for JWT mode.
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

        group.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok(new { message = "Logout successful" });
        }).RequireAuthorization();

        group.MapGet("/me", (ClaimsPrincipal user, UserManager<IdentityUser> userManager) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = user.FindFirstValue(ClaimTypes.Email);
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Results.Ok(new UserInfoResponse(userId, email, roles));
        }).RequireAuthorization();
    }
}

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password, bool RememberMe = false);

public record UserInfoResponse(string? Id, string? Email, List<string> Roles);

// Used by the JWT /token endpoint
public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
