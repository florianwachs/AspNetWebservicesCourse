using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Services
{
    public interface IBookCatalogService
    {
        Task<IEnumerable<Book>> GetBooks();
    }
}