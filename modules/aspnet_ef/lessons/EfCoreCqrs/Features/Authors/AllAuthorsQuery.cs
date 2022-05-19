using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.Features.Authors;

public record AllAuthorsQuery : IRequest<ApiResult<IReadOnlyCollection<AllAuthorVm>>>;

public record AllAuthorVm
{
    // Der Einfachheit halber nehmen wir für die Authoren die DB-Id her,
    // in der Regel sollte man keine DB-Ids herausgeben, besser wie bei den Büchern eine "public-id" wie ISBN verwenden
    public int Id { get; set; }
    public string Name { get; set; }

    public static AllAuthorVm From(Author s)
    {
        return new AllAuthorVm
        {
            Id = s.Id,
            Name = $"{s.FirstName} {s.LastName}",
        };
    }    
}

public class AllAuthorsQueryHandler : IRequestHandler<AllAuthorsQuery, ApiResult<IReadOnlyCollection<AllAuthorVm>>>
{
    private readonly ILogger<AllAuthorsQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public AllAuthorsQueryHandler(ILogger<AllAuthorsQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<IReadOnlyCollection<AllAuthorVm>>> Handle(AllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _dbContext.Authors.ToListAsync();
        var viewModels = authors.Select(a => AllAuthorVm.From(a)).ToArray();

        return ApiResult<IReadOnlyCollection<AllAuthorVm>>.Successful(viewModels);
    }
}

