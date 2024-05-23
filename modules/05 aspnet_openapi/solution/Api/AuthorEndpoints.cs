using EfCoreCqrs.Domain;
using EfCoreCqrs.Features.Authors;
using MediatR;

namespace EfCoreCqrs.Api;

public static class AuthorEndpoints
{
    public static IEndpointRouteBuilder MapAuthors(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/authors", HandleAllAuthors)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IEnumerable<Author>>(StatusCodes.Status200OK)
            .WithTags("Authors")
            .WithSummary("Returns all authors")
            .WithOpenApi();
        
        app.MapGet("/api/v1/authors/{id}", HandleAuthorById)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<Author>(StatusCodes.Status200OK)
            .WithTags("Authors")
            .WithSummary("Returns a specific author by id")
            .WithOpenApi(op =>
            {
                var idParam = op.Parameters[0];
                idParam.Name = "Id";
                idParam.Required = true;
                idParam.Description = "The Id of the author";
                return op;
            });

        return app;
    }

    public static async Task<IResult> HandleAllAuthors(IMediator mediator)
    {
        var result = await mediator.Send(new AllAuthorsQuery());
        return result.ToIResult();
    }

    public static async Task<IResult> HandleAuthorById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new AuthorDetailsQuery(id));
        return result.ToIResult();
    }
}
