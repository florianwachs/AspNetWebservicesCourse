using System.Collections.Generic;
using System.Threading.Tasks;

namespace graphqlservice.Books
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> All();
        Task<Book> GetById(string id);
    }
}