using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record BookDetailsQuery(string Isbn) : IRequest<ApiResult<BookDetailsVm>>;
public record BookDetailsVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";
    public DateTime? ReleaseDate { get; init; }
    public decimal? Price { get; init; }
    public string Rating { get; init; } = "";
    public IReadOnlyCollection<string> Authors { get; init; } = Array.Empty<string>();

    public static BookDetailsVm From(Book s)
    {
        return new BookDetailsVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
            ReleaseDate = s.ReleaseDate,
            Price = s.Price,
            Authors = s.Authors?.Select(a => a.LastName).ToArray() ?? Array.Empty<string>(),
            Rating = s.Rating switch
            {
                > 5 => "AWESOME",
                > 3 => "GOOD",
                _ => "MAYBE SOMETHING ELSE?"
            }
        };
    }
}

public class DetailBookQueryHandler : IRequestHandler<BookDetailsQuery, ApiResult<BookDetailsVm>>
{
    private readonly BookDbContext _dbContext;

    public DetailBookQueryHandler(ILogger<DetailBookQueryHandler> logger, BookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ApiResult<BookDetailsVm>> Handle(BookDetailsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Isbn))
        {
            return ApiResult<BookDetailsVm>.Failure("no isin provided");
        }

        var book = await _dbContext.Books
            .Include(a => a.Authors)
            .Where(b => b.Isbn == request.Isbn).FirstOrDefaultAsync();

        if (book == null)
        {
            return ApiResult<BookDetailsVm>.Failure($"no book found with isbn {request.Isbn}");
        }

        var vm = BookDetailsVm.From(book);

        return ApiResult<BookDetailsVm>.Successful(vm);
    }
}
