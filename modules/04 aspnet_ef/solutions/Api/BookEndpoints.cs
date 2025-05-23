﻿using EfCoreCqrs.Features.Books;
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
        app.MapPost("/api/v1/books", AddNewBook);
        app.MapPut("/api/v1/books/{isbn}/authors", AddAuthorToBook);
        app.MapDelete("/api/v1/books/{isbn}/authors/{authorId}", RemoveAuthorFromBook);
        app.MapDelete("/api/v1/books/{isbn}", RemoveBook);
        return app;
    }

    private static async Task RemoveBook(string isbn, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteBookCommand(isbn));
        
        
    }

    public static async Task<IResult> HandleTopRated(IMediator mediator, [FromQuery] int? top = 5)
    {
        var result = await mediator.Send(new TopBooksQuery() { Top = top.Value });
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

    public static async Task<IResult> AddNewBook(NewBookCommand request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.ToIResult();
    }

    public static async Task<IResult> AddAuthorToBook(AddAuthorToBookCommand request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.ToIResult();
    }

    public static async Task<IResult> RemoveAuthorFromBook(string isbn, int authorId, IMediator mediator)
    {
        var result = await mediator.Send(new RemoveAuthorFromBookCommand { AuthorId = authorId, BookIsbn = isbn });
        return result.ToIResult();
    }
}