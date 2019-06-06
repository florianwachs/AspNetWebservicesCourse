using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwaggerLesson.Models
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