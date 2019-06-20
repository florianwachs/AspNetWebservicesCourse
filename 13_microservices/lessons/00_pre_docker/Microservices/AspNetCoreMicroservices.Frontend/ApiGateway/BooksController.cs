using AspNetCoreMicroservices.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Frontend.ApiGateway
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        // Naiver Ansatz, besser: HttpClientFactory
        private static HttpClient _client = new HttpClient();
        private readonly IOptionsSnapshot<ApiConfig> _config;
        private readonly Uri _baseUri;

        public BooksController(IOptionsSnapshot<ApiConfig> config)
        {
            _config = config;
            _baseUri = new Uri(config.Value.BooksServiceBaseUri);
        }

        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var response = await _client.GetAsync(new Uri(_baseUri, "api/books"));
            response.EnsureSuccessStatusCode();

            return Ok(await response.Content.ReadAsAsync<IEnumerable<BookDto>>());
        }
    }
}
