using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class BookCatalogService : IBookCatalogService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<BookCatalogService> _logger;

        private readonly string _baseServiceUrl;

        public BookCatalogService(IOptions<AppSettings> settings, IHttpClient httpClient, ILogger<BookCatalogService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;
            _baseServiceUrl = $"{settings.Value.BookCatalogUrl}/api/v1/books";
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            var dataString = await _httpClient.GetStringAsync(_baseServiceUrl);
            if (string.IsNullOrWhiteSpace(dataString))
                return Array.Empty<Book>();

            var response = JsonConvert.DeserializeObject<IEnumerable<Book>>(dataString);
            return response;
        }
    }
}
