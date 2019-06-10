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
        // Ruft CreateClient() der Basisklasse auf
        var client = CreateClient();

        // Führt einen Request gegen die API In-Memory durch
        var response = await client.GetAsync("/api/containers/infos");

        // Wie beim echten HttpClient kann das Resultat überprüft und
        // verarbeitet werden.
        Assert.True(response.IsSuccessStatusCode);
        var infos = await response.Content.ReadAsAsync<IEnumerable<ContainerInfo>>();
        Assert.NotEmpty(infos);
    }
}
}
