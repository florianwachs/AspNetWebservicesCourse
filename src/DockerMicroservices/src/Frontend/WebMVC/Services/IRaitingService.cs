using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Services
{
    public interface IRaitingService
    {
        Task<RaitingResponse> GetRatingsForBooks(IEnumerable<Book> books);
    }
}