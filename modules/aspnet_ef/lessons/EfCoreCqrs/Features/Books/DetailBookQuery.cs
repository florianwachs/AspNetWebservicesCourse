using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record DetailBookQuery(string Isbn) : IRequest<ApiResult<DetailBookVm>>;
public record DetailBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";
    public DateTime? ReleaseDate { get; init; }
    public decimal? Price { get; init; }
    public string Rating { get; init; } = "";
    public IReadOnlyCollection<string> Authors { get; init; } = Array.Empty<string>();

    public static DetailBookVm From(Book s)
    {
        return new DetailBookVm()
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

public class DetailBookQueryHandler : IRequestHandler<DetailBookQuery, ApiResult<DetailBookVm>>
{
    private readonly BookDbContext _dbContext;

    public DetailBookQueryHandler(ILogger<DetailBookQueryHandler> logger, BookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ApiResult<DetailBookVm>> Handle(DetailBookQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Isbn))
        {
            return ApiResult<DetailBookVm>.Failure("no isin provided");
        }

        var book = await _dbContext.Books
            .Include(a => a.Authors)
            .Where(b => b.Isbn == request.Isbn).FirstOrDefaultAsync();

        if (book == null)
        {
            return ApiResult<DetailBookVm>.Failure($"no book found with isbn {request.Isbn}");
        }

        var vm = DetailBookVm.From(book);

        return ApiResult<DetailBookVm>.Successful(vm);
    }
}
