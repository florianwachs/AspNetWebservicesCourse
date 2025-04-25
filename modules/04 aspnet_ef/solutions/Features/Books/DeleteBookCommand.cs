using EfCoreCqrs.DataAccess;
using MediatR;

namespace EfCoreCqrs.Features.Books;

public record DeleteBookCommand(string Isbn) : IRequest<bool>;

public class DeleteBookCommandHandler(BookDbContext context) : IRequestHandler<DeleteBookCommand, bool>
{
    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = context.Books.FirstOrDefault(b => b.Isbn == request.Isbn);
        if (book == null)
        {
            return false;
        }
        context.Books.Remove(book);
        context.SaveChanges();
        return true;
    }
}