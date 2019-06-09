using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreUnitTests.Api.ContainerManagement.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AspNetCoreUnitTests.IntegrationTests.ContainerManagement
{
    public class ContainersControllerTests : IntegrationTestBase
    {
        public ContainersControllerTests(WebApplicationFactory<Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_be_able_to_get_all_container_infos()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/containers/infos");

            Assert.True(response.IsSuccessStatusCode);
            
            var infos = await response.Content.ReadAsAsync<IEnumerable<ContainerInfo>>();
            Assert.NotEmpty(infos);
        }
    }
}
