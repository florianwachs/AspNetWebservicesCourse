using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graphqlservice.Books
{
    public class BookRepository
    {
        private static readonly List<Book> _books = new List<Book>()
    {
        new Book{ Id = "book1", Name="C# in a Nutshell", Isbn="ISBN1", ReleaseDate= new DateTime(2020, 4, 20), Price=20},
        new Book{ Id = "book2", Name="Atomic Habits", Isbn="ISBN2", ReleaseDate= new DateTime(2010, 1, 1), Price=20},
        new Book{ Id = "book3", Name="Mindfulness", Isbn="ISBN3", ReleaseDate= new DateTime(2005, 3, 4), Price=11.5m}
    };

        public Task<IEnumerable<Book>> All() => Task.FromResult((IEnumerable<Book>)_books);
        public Task<Book> GetById(string id) => Task.FromResult(_books.FirstOrDefault(b => b.Id == id));

    }
}