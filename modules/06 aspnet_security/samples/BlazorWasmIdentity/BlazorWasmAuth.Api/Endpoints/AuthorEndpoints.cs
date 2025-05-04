using BlazorWasamAuth.Api.Auth;
using BlazorWasamAuth.Api.Providers;
using BlazorWasmAuth.Shared.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorWasamAuth.Api.Endpoints;

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
            .WithTags("Get-Operations")
            .WithOpenApi(op =>
            {
                op.Summary = "Returns all authors";
                op.Description = "Additional Description / Examples";

                return op;
            });

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
            .WithOpenApi(op =>
            {
                op.Summary = "Returns an author by id";
                op.Description = "Additional Description / Examples";
                
                var idParam = op.Parameters[0];                
                idParam.Required = true;
                idParam.Description = "The Id of the author";

                return op;
            })
            .WithName("GetAuthorById");

        group.MapDelete("{id:int}", (int id, DataProvider dataProvider) =>
            {
                dataProvider.Delete(id);
            }).WithTags("Delete-Operations")
            .WithOpenApi(op =>
            {
                op.Summary = "Deletes an author by id";
                op.Description = "Additional Description / Examples";

                var idParam = op.Parameters[0];
                idParam.Required = true;
                idParam.Description = "The Id of the author";

                return op;
            })
            .RequireAuthorization(AuthConstants.Policies.Admin);

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
            .WithOpenApi(op =>
            {
                op.Summary = "Creates a new author";
                op.Description = "Additional Description / Examples";
                var bodyDescription = op.RequestBody;
                bodyDescription.Description = "The data for the new author";
                bodyDescription.Required = true;

                return op;
            }).RequireAuthorization(AuthConstants.Policies.Admin);

        group.MapGet("/chuck/books", () =>
            {
                return Results.Ok(new Book[] { new Book() { Title = "Chucks Wisdom" } });
            })
            .Produces<List<Book>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi()
            .RequireAuthorization(AuthConstants.Policies.AllowedToReadChuckNorrisBooks);
    }
}