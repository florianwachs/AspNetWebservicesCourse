using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class PricingService : IPricingService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<PricingService> _logger;
        private string _baseServiceUrl;

        public PricingService(IOptions<AppSettings> settings, IHttpClient httpClient, ILogger<PricingService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;
            _baseServiceUrl = $"{settings.Value.PricingUrl}/api/v1/pricing/";
        }

        public async Task<PriceResponse> GetPricesForBooks(IEnumerable<Book> books)
        {
            books = books ?? throw new ArgumentException("books");
            var request = new PriceRequest { BookIds = books.Select(b => b.Id).ToArray() };
            var requestUrl = _baseServiceUrl + "bookprices";
            var response = await _httpClient.PostAsync(requestUrl, request);
            response.EnsureSuccessStatusCode();
            var prices = await response.Content.ReadAsAsync<PriceResponse>();
            return prices;
        }
    }
}
