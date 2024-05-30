using System.Security.Claims;

namespace BlazorWasamAuth.Api.Endpoints;

public static class CustomIdentityEndpoints
{
    public static void MapCustomIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/roles", (ClaimsPrincipal user) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c => 
                        new
                        {
                            c.Issuer, 
                            c.OriginalIssuer, 
                            c.Type, 
                            c.Value, 
                            c.ValueType
                        });

                return TypedResults.Json(roles);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();
        
        app.MapGet("/claims", (ClaimsPrincipal user) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var claims = identity.Claims
                    .Select(c => 
                        new
                        {
                            c.Issuer, 
                            c.OriginalIssuer, 
                            c.Type, 
                            c.Value, 
                            c.ValueType
                        });

                return TypedResults.Json(claims);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();
    }
}