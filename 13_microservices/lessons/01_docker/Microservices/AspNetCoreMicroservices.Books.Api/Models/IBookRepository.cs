using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Books.Api.Models
{
    public interface IBookRepository
    {
        Task<IReadOnlyCollection<Book>> GetAll();
        Task<Book> GetById(string id);
        Task<Book> Add(Book book);
        Task<Book> Update(Book book);
        Task Delete(string id);
    }
}