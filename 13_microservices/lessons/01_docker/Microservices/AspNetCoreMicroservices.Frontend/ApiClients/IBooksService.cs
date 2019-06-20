using AspNetCoreMicroservices.Frontend.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Frontend.ApiClients
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDto>> GetBooks();
    }

    public class BooksService : IBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;

        public BooksService(HttpClient httpClient, IOptionsSnapshot<ApiConfig> config)
        {
            _httpClient = httpClient;
            _baseUri = new Uri(config.Value.BooksServiceBaseUri);
        }

        public async Task<IEnumerable<BookDto>> GetBooks()
        {
            var response = await _httpClient.GetAsync(new Uri(_baseUri, "api/books"));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IEnumerable<BookDto>>();
        }
    }
}
