using AspNetCoreTesting.Api;
using AspNetCoreTesting.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests
{
    public class JokesTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private static JsonSerializerOptions DefaultOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private HttpClient HttpClient { get; }
        public JokesTests(WebApplicationFactory<Startup> applicationFactory)
        {
            HttpClient = applicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task CantCreateInvalidJoke()
        {
            var response = await HttpClient.PostAsJsonAsync("/api/v1/jokes", new Joke());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }     

        [Fact]
        public async Task CreateNewJoke()
        {
            var response = await HttpClient.PostAsJsonAsync("/api/v1/jokes", new Joke { JokeText = "This should be funny" });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        }     


    }
}
