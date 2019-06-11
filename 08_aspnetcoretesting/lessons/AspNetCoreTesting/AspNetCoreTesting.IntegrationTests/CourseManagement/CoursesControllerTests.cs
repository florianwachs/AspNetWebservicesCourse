using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests.CourseManagement
{
    public class CoursesControllerTests : IntegrationTestBase
    {
        public CoursesControllerTests(WebApplicationFactory<Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_return_all_courses()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/courses");
            Assert.True(response.IsSuccessStatusCode);

            var result = await response.Content.ReadAsAsync<IEnumerable<Course>>();
            Assert.NotEmpty(result);
        }

    }
}
