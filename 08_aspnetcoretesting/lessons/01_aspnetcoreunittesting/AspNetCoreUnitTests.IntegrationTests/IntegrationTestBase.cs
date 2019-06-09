using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreUnitTests.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AspNetCoreUnitTests.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTestBase(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        public HttpClient CreateClient() => _factory.CreateClient();
    }
}
