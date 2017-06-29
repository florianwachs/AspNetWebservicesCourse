using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebMVC.Services
{
    public class RaitingService : IRaitingService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<RaitingService> _logger;
        private string _baseServiceUrl;

        public RaitingService(IOptions<AppSettings> settings, IHttpClient httpClient, ILogger<RaitingService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;
            _baseServiceUrl = $"{settings.Value.RatingUrl}/api/v1/raitings/";
        }

        public async Task<RaitingResponse> GetRatingsForBooks(IEnumerable<Book> books)
        {
            books = books ?? throw new ArgumentException("books");
            var requestUrl = _baseServiceUrl + "bookratings";
            var ratingRequest = new RaitingRequest { BookIds = books.Select(b => b.Id).ToArray() };
            var response = await _httpClient.PostAsync(requestUrl, ratingRequest);
            var rating = await response.Content.ReadAsAsync<RaitingResponse>();
            return rating;
        }

    }
}
