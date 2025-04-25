using EfCoreCqrs.DataAccess;
using EfCoreCqrs.Domain;
using MediatR;

namespace EfCoreCqrs.Features.Authors;

public record AddAuthorCommand: IRequest<Author>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}


public class AddAuthorCommandHandler(BookDbContext context) : IRequestHandler<AddAuthorCommand, Author>
{
    public async Task<Author> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
    {
        // Never Trust the Client => Validation

        var newAutor = new Author()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age
        };
        context.Authors.Add(newAutor);

        await context.SaveChangesAsync();

        // Careful, probably better to return a dto instead of the entity
        return newAutor;
    }
}
