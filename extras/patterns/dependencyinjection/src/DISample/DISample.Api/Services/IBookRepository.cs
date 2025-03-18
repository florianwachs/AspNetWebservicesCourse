namespace DISample.Api.Services;

public interface IBookRepository
{
    Book GetBookById(string id);
    Book Add(Book book);
    IReadOnlyCollection<Book> All();
}

public class Book
{
    // ...
}
