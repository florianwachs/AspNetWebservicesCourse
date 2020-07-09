using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISample.Api.Services
{
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
}
