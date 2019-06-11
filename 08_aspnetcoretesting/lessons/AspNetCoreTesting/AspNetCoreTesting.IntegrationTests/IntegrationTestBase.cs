using System.Net.Http;
using AspNetCoreTesting.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTestBase(WebApplicationFactory<Startup> factory) => _factory = factory;

        public HttpClient CreateClient() => _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Hier können Services aus der normalen Startup der App mit
                // Fake / Mock-Services überschrieben werden
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    // Hiermit kann das DI-System hochgefahren werden
                    // und z.B. eine In-Memory-DB mit Daten gefüllt werden.
                    var scopedServices = scope.ServiceProvider;
                    //var db = scopedServices
                    //    .GetRequiredService<ApplicationDbContext>();
                }
            });

            builder.ConfigureServices(services =>
            {
                // Hier können zusätzliche Services konfiguriert werden.
            });
        }).CreateClient();
    }
}
