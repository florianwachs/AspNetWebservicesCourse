using EfCoreCqrs.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record AddAuthorToBookCommand : IRequest<ApiResult<bool>>
{
    public string? BookIsbn { get; init; }
    public int? AuthorId { get; init; }
}

public class AddAuthorToBookCommandHandler : IRequestHandler<AddAuthorToBookCommand, ApiResult<bool>>
{
    private readonly ILogger<AddAuthorToBookCommand> _logger;
    private readonly BookDbContext _dbContext;

    public AddAuthorToBookCommandHandler(ILogger<AddAuthorToBookCommand> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<ApiResult<bool>> Handle(AddAuthorToBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _dbContext.Books
            .Include(b=>b.Authors)
            .Where(b => b.Isbn == request.BookIsbn)
            .FirstOrDefaultAsync();

        var author = await _dbContext.Authors.FindAsync(request.AuthorId);

        if(book is null)
        {
            return ApiResult<bool>.Failure($"No book found with ISBN {request.BookIsbn}");
        }

        if (author is null)
        {
            return ApiResult<bool>.Failure($"No author found with Id {request.AuthorId}");
        }

        book.Authors.Add(author);
        await _dbContext.SaveChangesAsync();

        return ApiResult<bool>.Successful(true);
    }
}

