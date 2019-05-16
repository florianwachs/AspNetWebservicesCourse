using AspNetCore.BooksServer.Models;
using System.Collections.Generic;

namespace AspNetCore.BooksServer.Repositories
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();
        Book GetById(int id);
        Book Add(Book p);
        Book Update(int id, Book p);
        bool Delete(int id);
    }
}
