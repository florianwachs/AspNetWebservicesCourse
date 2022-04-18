using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISample.Api.Services
{
    public class DummyBookRepository : IBookRepository
    {
        public Book Add(Book book)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Book> All()
        {
            return Array.Empty<Book>();
        }

        public Book GetBookById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
