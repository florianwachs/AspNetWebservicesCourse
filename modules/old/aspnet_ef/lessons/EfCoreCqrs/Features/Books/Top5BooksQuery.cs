using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record Top3BooksQuery : IRequest<ApiResult<IReadOnlyCollection<TopBookVm>>>;
public record TopBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";

    public static TopBookVm From(Book s)
    {
        return new TopBookVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
        };
    }
}

public class Top3BooksQueryHandler : IRequestHandler<Top3BooksQuery, ApiResult<IReadOnlyCollection<TopBookVm>>>
{
    private readonly ILogger<Top3BooksQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public Top3BooksQueryHandler(ILogger<Top3BooksQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<IReadOnlyCollection<TopBookVm>>> Handle(Top3BooksQuery request, CancellationToken cancellationToken)
    {
        var candidates = await _dbContext.Books.OrderByDescending(b => b.Rating).Take(3).ToListAsync();
        var viewModels = candidates.Select(c => TopBookVm.From(c)).ToArray();

        return ApiResult<IReadOnlyCollection<TopBookVm>>.Successful(viewModels);
    }
}

