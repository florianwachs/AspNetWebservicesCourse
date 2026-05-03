using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", RegisterAsync)
            .WithSummary("Register a new speaker account")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapPost("/token", LoginAsync)
            .WithSummary("Exchange email and password for a JWT")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization()
            .WithSummary("Return the current authenticated user");
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        UserManager<IdentityUser> userManager,
        AppDbContext db,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            throw new ConflictException($"A user with email '{request.Email}' already exists.");
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Results.ValidationProblem(createResult.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(group => group.Key, group => group.Select(x => x.Description).ToArray()));
        }

        await userManager.AddToRoleAsync(user, RoleNames.Speaker);
        await userManager.AddClaimAsync(user, new Claim(AppClaimTypes.SpeakerProfileWrite, "true"));
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, request.DisplayName));

        db.SpeakerProfiles.Add(new SpeakerProfile
        {
            UserId = user.Id,
            DisplayName = request.DisplayName,
            Tagline = request.Tagline,
            Bio = request.Bio,
            Company = request.Company,
            City = request.City,
            Email = request.Email,
            WebsiteUrl = request.WebsiteUrl,
            PhotoUrl = request.PhotoUrl,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);

        var response = await CreateAuthResponseAsync(user, userManager, jwtOptions.Value, db, cancellationToken);
        return TypedResults.Created("/api/auth/me", response);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        UserManager<IdentityUser> userManager,
        AppDbContext db,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.Problem(
                title: "Unauthorized",
                detail: "Invalid email or password.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var response = await CreateAuthResponseAsync(user, userManager, jwtOptions.Value, db, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        UserManager<IdentityUser> userManager,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            throw new NotFoundException("User", principal.Identity?.Name ?? "current");
        }

        var roles = await userManager.GetRolesAsync(user);
        var claims = await userManager.GetClaimsAsync(user);
        var speakerProfileId = await db.SpeakerProfiles
            .Where(x => x.UserId == user.Id)
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return TypedResults.Ok(new CurrentUserResponse(
            user.Id,
            user.Email ?? string.Empty,
            roles.ToArray(),
            claims.Select(x => new UserClaimResponse(x.Type, x.Value)).ToArray(),
            speakerProfileId));
    }

    private static async Task<AuthResponse> CreateAuthResponseAsync(
        IdentityUser user,
        UserManager<IdentityUser> userManager,
        JwtOptions jwtOptions,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = await userManager.GetClaimsAsync(user);
        var speakerProfileId = await db.SpeakerProfiles
            .Where(x => x.UserId == user.Id)
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);

        var tokenClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };

        var displayNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        if (displayNameClaim is not null)
        {
            tokenClaims.Add(displayNameClaim);
        }

        tokenClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        tokenClaims.AddRange(claims.Where(x => x.Type != ClaimTypes.Name));

        var credentials = new SigningCredentials(jwtOptions.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: tokenClaims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse(
            accessToken,
            "Bearer",
            (int)(expiresAt - now).TotalSeconds,
            new CurrentUserResponse(
                user.Id,
                user.Email ?? string.Empty,
                roles.ToArray(),
                claims.Select(x => new UserClaimResponse(x.Type, x.Value)).ToArray(),
                speakerProfileId));
    }
}

public sealed record RegisterRequest(
    [property: FromBody] string Email,
    string Password,
    string DisplayName,
    string Tagline,
    string Bio,
    string Company,
    string City,
    string? WebsiteUrl,
    string? PhotoUrl);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    CurrentUserResponse User);

public sealed record CurrentUserResponse(
    string Id,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<UserClaimResponse> Claims,
    int? SpeakerProfileId);

public sealed record UserClaimResponse(string Type, string Value);
