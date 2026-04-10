using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace TechConf.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app, bool useJwtAuth)
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

            var addRoleResult = await userManager.AddToRoleAsync(user, "attendee");
            if (!addRoleResult.Succeeded)
            {
                return Results.ValidationProblem(
                    addRoleResult.Errors.GroupBy(e => e.Code).ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()));
            }

            return Results.Ok(new { message = "Registration successful" });
        });

        if (useJwtAuth)
        {
            group.MapPost("/login", async (LoginRequest request, UserManager<IdentityUser> userManager, IConfiguration config) =>
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Results.Problem("Invalid email or password.", statusCode: 401);
                }

                var roles = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(ClaimTypes.Email, user.Email ?? request.Email)
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(
                        config["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key configuration.")));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                    issuer: config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer configuration."),
                    audience: config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience configuration."),
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials);

                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
                return Results.Ok(new TokenResponse(jwt, "Bearer", 7200));
            });

            group.MapPost("/logout", () =>
                Results.Ok(new { message = "JWT logout is client-side. Discard the bearer token." }));
        }
        else
        {
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

            group.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok(new { message = "Logout successful" });
            }).RequireAuthorization();
        }

        group.MapGet("/me", (ClaimsPrincipal user) =>
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

public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
