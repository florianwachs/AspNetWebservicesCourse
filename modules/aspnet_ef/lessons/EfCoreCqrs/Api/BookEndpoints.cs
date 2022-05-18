using EfCoreCqrs.Features.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreCqrs.Api;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBooks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/books", HandleAllBooks);
        app.MapGet("/api/v1/books/{isbn}", GetBookByIsbn);
        app.MapGet("/api/v1/books/searches/simple", SearchBooks);
        app.MapGet("/api/v1/books/toprated", HandleTopRated);

        return app;
    }

    public static async Task<IResult> HandleTopRated(IMediator mediator)
    {
        var result = await mediator.Send(new Top3BooksQuery());
        return result.ToIResult();
    }

    public static async Task<IResult> HandleAllBooks(IMediator mediator)
    {
        var result = await mediator.Send(new AllBooksQuery());
        return result.ToIResult();
    }

    public static async Task<IResult> SearchBooks([FromQuery] string? q, IMediator mediator)
    {
        var result = await mediator.Send(new SearchBooksQuery(q));
        return result.ToIResult();
    }

    public static async Task<IResult> GetBookByIsbn(string isbn, IMediator mediator)
    {
        var result = await mediator.Send(new BookDetailsQuery(isbn));
        return result.ToIResult();
    }
}
