using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;
using AspNetCoreSecurity.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests.CourseManagement
{

    public class ProfessorControllerTests : IntegrationTestBase
    {
        public ProfessorControllerTests(WebApplicationFactory<AspNetCoreSecurity.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_return_NotFound_for_unknown_professor()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/professors/dontexists/assignedcourses");
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
