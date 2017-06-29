using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Services
{
    public interface IRatingService
    {
        Task<RatingResponse> GetRatingsForBooks(IEnumerable<Book> books);
    }
}