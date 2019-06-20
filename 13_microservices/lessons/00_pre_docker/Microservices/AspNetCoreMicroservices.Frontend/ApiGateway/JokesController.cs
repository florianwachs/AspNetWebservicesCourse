using AspNetCoreMicroservices.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Frontend.ApiGateway
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokesController : ControllerBase
    {
        // Naiver Ansatz, besser: HttpClientFactory
        private static readonly HttpClient _client = new HttpClient();
        private readonly IOptionsSnapshot<ApiConfig> _config;
        private readonly Uri _baseUri;

        public JokesController(IOptionsSnapshot<ApiConfig> config)
        {
            _config = config;
            _baseUri = new Uri(config.Value.JokesServiceBaseUri);
        }

        public async Task<ActionResult<IEnumerable<JokeDto>>> GetBooks()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(_baseUri, "api/jokes"));
            response.EnsureSuccessStatusCode();

            return Ok(await response.Content.ReadAsAsync<IEnumerable<JokeDto>>());
        }
    }
}
