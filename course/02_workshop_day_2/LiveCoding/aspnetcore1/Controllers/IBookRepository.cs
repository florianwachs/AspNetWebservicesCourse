using System.Collections.Generic;

namespace aspnetcore1.Controllers
{
    public interface IBookRepository
    {
        Book Add(Book book);
        IEnumerable<Book> GetAll();
        Book GetById(int id);
    }
}