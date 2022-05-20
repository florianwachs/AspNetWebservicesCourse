using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using EfCoreCqrs.Features.Books;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreCqrs.Features.Authors;

public record AddAuthorCommand : IRequest<ApiResult<NewAuthorVm>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public record NewAuthorVm
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, ApiResult<NewAuthorVm>>
{
    private readonly BookDbContext _dbContext;

    public AddAuthorCommandHandler(BookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResult<NewAuthorVm>> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ApiResult<NewAuthorVm>.Failure(validationResult.ToString());
        }

        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
        };

        await _dbContext.AddAsync(author);
        await _dbContext.SaveChangesAsync();

        return ApiResult<NewAuthorVm>.Successful(new NewAuthorVm
        {
            FirstName = author.FirstName,
            LastName = author.LastName
        });

    }

    public class Validator : AbstractValidator<AddAuthorCommand>
    {
        public Validator()
        {
            RuleFor(a => a.FirstName).NotEmpty().MaximumLength(1000);
            RuleFor(a => a.LastName).NotEmpty().MaximumLength(1000);
            RuleFor(a => a.Age).InclusiveBetween(1, 140);
        }
    }
}


