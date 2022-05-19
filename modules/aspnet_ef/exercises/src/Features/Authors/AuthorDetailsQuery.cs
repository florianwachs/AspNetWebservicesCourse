using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;

namespace EfCoreCqrs.Features.Authors;

public record AuthorDetailsQuery(int Id) : IRequest<ApiResult<AuthorDetailsVm>>;

public record AuthorDetailsVm
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public int Age { get; init; }

    public static AuthorDetailsVm From(Author s)
    {
        return new()
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Age = s.Age,
        };
    }
}

public class AuthorDetailsQueryHandler : IRequestHandler<AuthorDetailsQuery, ApiResult<AuthorDetailsVm>>
{
    private readonly ILogger<AuthorDetailsQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public AuthorDetailsQueryHandler(ILogger<AuthorDetailsQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<AuthorDetailsVm>> Handle(AuthorDetailsQuery request, CancellationToken cancellationToken)
    {
        var author = await _dbContext.Authors.FindAsync(request.Id);
        if (author == null)
        {
            return ApiResult<AuthorDetailsVm>.Failure($"no book found with isbn {request.Id}");
        }

        return ApiResult<AuthorDetailsVm>.Successful(AuthorDetailsVm.From(author));

    }
}
