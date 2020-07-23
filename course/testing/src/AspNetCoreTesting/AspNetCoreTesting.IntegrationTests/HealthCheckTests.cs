using AspNetCoreTesting.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; }
        public HealthCheckTests(WebApplicationFactory<Startup> applicationFactory)
        {
            HttpClient = applicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheckReturnsHealthy()
        {
            var response = await HttpClient.GetAsync("/healthcheck");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
