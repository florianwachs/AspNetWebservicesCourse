using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public class NewBookCommand : IRequest<ApiResult<NewBookResult>>
{
    public string Title { get; init; }
    public string Isbn { get; init; }
    public decimal? Price { get; init; }
    public IReadOnlyCollection<int> Authors { get; init; } = Array.Empty<int>();
}

public record NewBookResult
{
    public string Title { get; init; }
    public string Isbn { get; init; }
}

public class NewBookCommandHandler : IRequestHandler<NewBookCommand, ApiResult<NewBookResult>>
{
    private readonly ILogger<NewBookCommandHandler> _logger;
    private readonly BookDbContext _bookDbContext;

    public NewBookCommandHandler(ILogger<NewBookCommandHandler> logger, BookDbContext bookDbContext)
    {
        _logger = logger;
        _bookDbContext = bookDbContext;
    }

    public async Task<ApiResult<NewBookResult>> Handle(NewBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = new Validator().Validate(request);
        if (!validationResult.IsValid)
        {
            return ApiResult<NewBookResult>.Failure(validationResult.ToString());
        }

        var existingBook = await _bookDbContext.Books.Where(b => b.Isbn == request.Isbn).FirstOrDefaultAsync();
        if (existingBook != null)
        {
            return ApiResult<NewBookResult>.Failure($"Book with ISBN {request.Isbn} already exists");
        }

        var authors = await GetAuthorsToAdd(request);

        var newBook = new Book()
        {
            Isbn = request.Isbn,
            Price = request.Price,
            Title = request.Title,
            Authors = authors,
        };

        try
        {
            _bookDbContext.Books.Add(newBook);
            await _bookDbContext.SaveChangesAsync();

            return ApiResult<NewBookResult>.Successful(new() { Isbn = newBook.Isbn, Title = newBook.Title });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add new book {@BookRequest}", request);
            return ApiResult<NewBookResult>.Failure(ex.ToString());
        }

    }

    private async Task<ICollection<Author>?> GetAuthorsToAdd(NewBookCommand request)
    {
        if (request.Authors?.Any() != true)
        {
            return null;
        }

        return await _bookDbContext.Authors.Where(a => request.Authors.Contains(a.Id)).ToListAsync();
    }


    public class Validator : AbstractValidator<NewBookCommand>
    {
        public Validator()
        {
            RuleFor(b => b.Title).NotEmpty();
            RuleFor(b => b.Isbn).NotEmpty();
        }

    }
}
