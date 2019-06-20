using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests.StudentManagement
{
    public class StudentsControllerTests : IntegrationTestBase
    {
        public StudentsControllerTests(WebApplicationFactory<Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_be_able_to_get_all_students()
        {
            // Ruft CreateClient() der Basisklasse auf
            var client = CreateClient();

            // Führt einen Request gegen die API In-Memory durch
            var response = await client.GetAsync("/api/students");

            // Wie beim echten HttpClient kann das Resultat überprüft und
            // verarbeitet werden.
            Assert.True(response.IsSuccessStatusCode);
            var students = await response.Content.ReadAsAsync<IEnumerable<Student>>();
            Assert.NotEmpty(students);
        }
    }
}
