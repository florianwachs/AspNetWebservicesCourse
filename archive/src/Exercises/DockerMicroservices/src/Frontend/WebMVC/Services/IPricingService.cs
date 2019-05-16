using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Services
{
    public interface IPricingService
    {
        Task<PriceResponse> GetPricesForBooks(IEnumerable<Book> books);
    }
}