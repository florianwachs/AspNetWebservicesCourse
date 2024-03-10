using EfCoreCqrs.Features.Authors;
using MediatR;

namespace EfCoreCqrs.Api;

public static class AuthorEndpoints
{
    public static IEndpointRouteBuilder MapAuthors(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/authors", HandleAllAuthors);
        app.MapGet("/api/v1/authors/{id}", HandleAuthorById);

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
