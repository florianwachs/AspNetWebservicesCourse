using EfCoreCqrs.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record RemoveAuthorFromBookCommand : IRequest<ApiResult<bool>>
{
    public string BookIsbn { get; init; }
    public int AuthorId { get; init; }
}

public class RemoveAuthorFromBookCommandHandler : IRequestHandler<RemoveAuthorFromBookCommand, ApiResult<bool>>
{
    private readonly ILogger<RemoveAuthorFromBookCommand> _logger;
    private readonly BookDbContext _dbContext;

    public RemoveAuthorFromBookCommandHandler(ILogger<RemoveAuthorFromBookCommand> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<ApiResult<bool>> Handle(RemoveAuthorFromBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _dbContext
            .Books
            .Include(b=>b.Authors)
            .Where(b => b.Isbn == request.BookIsbn).FirstOrDefaultAsync();
        var author = await _dbContext.Authors.FindAsync(request.AuthorId);

        if(book is null)
        {
            return ApiResult<bool>.Failure($"No book found with ISBN {request.BookIsbn}");
        }

        if (author is null)
        {
            return ApiResult<bool>.Failure($"No author found with Id {request.AuthorId}");
        }

        book.Authors.Remove(author);
        await _dbContext.SaveChangesAsync();

        return ApiResult<bool>.Successful(true);
    }
}

