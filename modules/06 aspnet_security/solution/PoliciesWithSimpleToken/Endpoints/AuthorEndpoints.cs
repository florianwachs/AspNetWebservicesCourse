using Microsoft.AspNetCore.Http.HttpResults;
using PoliciesWithSimpleToken.Auth;
using PoliciesWithSimpleToken.Domain;
using PoliciesWithSimpleToken.Providers;

namespace PoliciesWithSimpleToken.Endpoints;

public static class AuthorEndpoints
{
    public static void MapAuthors(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/authors").WithTags("Authors");

        group.MapGet("", async Task<Ok<List<Author>>> (DataProvider provider) =>
            {
                var authors = await provider.GetAuthors();
                return TypedResults.Ok(authors);
            })
            .RequireAuthorization()
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Get-Operations");

        group.MapGet("{id:int}",
                async Task<Results<Ok<Author>, NotFound>> (int id, DataProvider provider) =>
                {
                    var author = await provider.GetAuthorById(id);
                    if (author == null)
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Ok(author);
                })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Get-Operations")
            .WithName("GetAuthorById");

        group.MapPost("",
                async Task<Results<Created<Author>, BadRequest>> (Author author, DataProvider provider,
                    HttpContext httpContext,
                    LinkGenerator linkGenerator) =>
                {
                    // Code to add an author
                    if (Random.Shared.NextDouble() > 0.5)
                    {
                        return TypedResults.BadRequest();
                    }

                    var uriToCreatedAuthor = linkGenerator.GetUriByName(httpContext, "AuthorById");

                    return TypedResults.Created(uriToCreatedAuthor, author);
                })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Post-Operations")
            .RequireAuthorization(AuthConstants.Policies.Admin);

        group.MapGet("/chuck/books",
                () => { return Results.Ok(new Book[] { new Book() { Title = "Chucks Wisdom" } }); })
            .Produces<List<Book>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthConstants.Policies.AllowedToReadChuckNorrisBooks);

        group.MapDelete("{id:int}", (int id) => { return Results.NoContent(); })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthConstants.Policies.CanDeleteAuthor);
    }
}