using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record TopBooksQuery : IRequest<ApiResult<IReadOnlyCollection<TopBookVm>>>
{
    public int? Limit { get; set; }
}
public record TopBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";
    public IReadOnlyCollection<string> Authors { get; set; }

    public static TopBookVm From(Book s)
    {
        return new TopBookVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
            Authors = s.Authors.Select(a=>a.LastName).ToList()
        };
    }
}

public class TopBooksQueryHandler : IRequestHandler<TopBooksQuery, ApiResult<IReadOnlyCollection<TopBookVm>>>
{
    private readonly ILogger<TopBooksQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public TopBooksQueryHandler(ILogger<TopBooksQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<IReadOnlyCollection<TopBookVm>>> Handle(TopBooksQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit ?? 3, 1, 50);
        var candidates = await _dbContext.Books
            .Include(b=>b.Authors)
            .OrderByDescending(b => b.Rating).Take(limit).ToListAsync();
        var viewModels = candidates.Select(c => TopBookVm.From(c)).ToArray();

        return ApiResult<IReadOnlyCollection<TopBookVm>>.Successful(viewModels);
    }
}

