using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record TopBooksQuery : IRequest<ApiResult<IReadOnlyCollection<TopBookVm>>>
{
    public int Top { get; init; } = 5;
}
public record TopBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";
    public string Authors { get; init; } = "";

    public static TopBookVm From(Book s)
    {
        var authors = string.Join(", ", (s.Authors ?? Enumerable.Empty<Author>()).Select(a=>$"{a.FirstName} {a.LastName}"));
        return new TopBookVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
            Authors = authors,
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
        var candidates = await _dbContext.Books
            .Include(b=>b.Authors)
            .OrderByDescending(b => b.Rating)
            .Take(request.Top)
            .ToListAsync();
        
        var viewModels = candidates.Select(c => TopBookVm.From(c)).ToArray();

        return ApiResult<IReadOnlyCollection<TopBookVm>>.Successful(viewModels);
    }
}

