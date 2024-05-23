using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace EfCoreCqrs.Features.Books;

public record SearchBooksQuery(string? Query) : IRequest<ApiResult<IReadOnlyCollection<SearchBookVm>>>;
public record SearchBookVm
{
    public string Isbn { get; init; } = "";
    public string Title { get; init; } = "";

    public static SearchBookVm From(Book s)
    {
        return new SearchBookVm()
        {
            Isbn = s.Isbn,
            Title = s.Title,
        };
    }
}

public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, ApiResult<IReadOnlyCollection<SearchBookVm>>>
{
    private readonly ILogger<SearchBooksQueryHandler> _logger;
    private readonly BookDbContext _dbContext;

    public SearchBooksQueryHandler(ILogger<SearchBooksQueryHandler> logger, BookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ApiResult<IReadOnlyCollection<SearchBookVm>>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var validationResult = new Validator().Validate(request);
        if (!validationResult.IsValid)
        {
            return ApiResult<IReadOnlyCollection<SearchBookVm>>.Failure(validationResult.ToString());
        }

        // 👇 Funktioniert leider so nicht
        //var candidates = await _dbContext.Books.Where(b => b.Title.Contains(request.Query!, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        var candidates = await _dbContext.Books.Where(b => EF.Functions.Like(b.Title, $"%{request.Query}%")).ToListAsync();


        var viewModels = candidates.Select(c => SearchBookVm.From(c)).ToArray();

        return ApiResult<IReadOnlyCollection<SearchBookVm>>.Successful(viewModels);
    }

    public class Validator : AbstractValidator<SearchBooksQuery>
    {
        public Validator()
        {
            RuleFor(q => q.Query).NotEmpty().MinimumLength(3).MaximumLength(100);
        }
    }
}

