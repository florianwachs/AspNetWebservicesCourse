using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore1.Controllers
{
    public class BookRepository : IBookRepository
    {
        private List<Book> _books;

        public BookRepository()
        {
            _books = new List<Book>() {
            new Book {Id=1, Name = "Test1", Isbn = "ISBN1" } ,
            new Book {Id=2, Name = "Flo wann ist endlich schluss?", Isbn = "ISBN2" }
                    };

        }

        public IEnumerable<Book> GetAll()
        {
            return _books;
        }

        public Book GetById(int id)
        {
            return _books.Where(book => book.Id == id).FirstOrDefault();
        }

        public Book Add(Book book)
        {
            _books.Add(book);
            return book;
        }
    }
}
