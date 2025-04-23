using EfCoreCqrs.Domain;
using EfCoreCqrs.Features.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreCqrs.Api;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBooks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/books", HandleAllBooks)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IEnumerable<Book>>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithSummary("Returns all books")
            .WithOpenApi();
        
        app.MapGet("/api/v1/books/{isbn}", GetBookByIsbn)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<Book>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithSummary("Returns a specific author by id")
            .WithOpenApi(op =>
            {
                var isbn = op.Parameters[0];
                isbn.Name = "Isbn";
                isbn.Required = true;
                isbn.Description = "The Isbn of the author";
                return op;
            });
        
        app.MapGet("/api/v1/books/searches/simple", SearchBooks)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IEnumerable<Book>>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithSummary("Returns books that match a simple search query")
            .WithOpenApi();
        
        app.MapGet("/api/v1/books/toprated", HandleTopRated)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IEnumerable<Book>>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithSummary("Returns all top rated books")
            .WithOpenApi();
        
        app.MapPost("/api/v1/books", AddNewBook)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<Book>(StatusCodes.Status201Created)
            .WithTags("Books")
            .WithOpenApi(op =>
            {
                op.Summary = "Creates a new book";
                op.Description = "Additional Description / Examples";
                var bodyDescription = op.RequestBody;
                bodyDescription.Description = "The data for the new book";
                bodyDescription.Required = true;

                return op;
            });
        
        app.MapPut("/api/v1/books/{isbn}/authors", AddAuthorToBook)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<Book>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithOpenApi(op =>
            {
                op.Summary = "Creates a new book";
                op.Description = "Additional Description / Examples";
                var bodyDescription = op.RequestBody;
                bodyDescription.Description = "The data for the new book";
                bodyDescription.Required = true;

                var isbn = op.Parameters[0];
                isbn.Name = "Isbn";
                isbn.Required = true;
                isbn.Description = "The Isbn of the author";
                return op;
            });
        
        app.MapDelete("/api/v1/books/{isbn}/authors/{authorId}", RemoveAuthorFromBook)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<Book>(StatusCodes.Status200OK)
            .WithTags("Books")
            .WithOpenApi(op =>
            {
                op.Summary = "Removed author from book";
                
                var isbn = op.Parameters[0];
                isbn.Name = "Isbn";
                isbn.Required = true;
                isbn.Description = "The Isbn of the author";
                
                var authorId = op.Parameters[1];
                authorId.Name = "Author Id";
                authorId.Required = true;
                authorId.Description = "The Id of the author thats need to be removed from the book";
                return op;
            });

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

    public static async Task<IResult> AddNewBook(NewBookCommand request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.ToIResult();
    }

    public static async Task<IResult> AddAuthorToBook(AddAuthorToBookCommand request,string isbn, IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.ToIResult();
    }

    public static async Task<IResult> RemoveAuthorFromBook(string isbn, int authorId, IMediator mediator)
    {
        var result = await mediator.Send(new RemoveAuthorFromBookCommand { AuthorId = authorId, BookIsbn = isbn});
        return result.ToIResult();
    }
}
