using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Books;

public record AllBooksQuery : IRequest<ApiResult<IReadOnlyCollection<AllBookVm>>>;
public record AllBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";
    public decimal Price { get; set; } 

    public static AllBookVm From(Book s)
    {
        return new AllBookVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
            Price = s.Price ?? 0m
        };
    }
}

public class AllBooksQueryHandler : IRequestHandler<AllBooksQuery, ApiResult<IReadOnlyCollection<AllBookVm>>>
{
    private readonly ILogger<AllBooksQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public AllBooksQueryHandler(ILogger<AllBooksQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<IReadOnlyCollection<AllBookVm>>> Handle(AllBooksQuery request, CancellationToken cancellationToken)
    {
        // keine gute Idee wenn man Amazon ist, hier wäre Paging notwendig
        var alleBücher = await _dbContext.Books.ToListAsync();
        var viewModels = alleBücher.Select(c => AllBookVm.From(c)).ToArray();

        return ApiResult<IReadOnlyCollection<AllBookVm>>.Successful(viewModels);
    }
}
