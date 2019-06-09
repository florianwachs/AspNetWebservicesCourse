using AspNetCoreUnitTests.Domain.Models;
using Xunit;

namespace AspNetCoreUnitTests.UnitTests.Containers
{
    public class EuContainerTests
    {
        [Fact]
        public void Should_create_new_container()
        {
            // SUT = System Under Test
            var sut = EuStandardContainer.New();

            // Die Assert-Klasse erlaubt die Validierung des erwarteten Verhaltens
            // Bei Nichterfüllung schlägt der Test fehl.
            Assert.NotNull(sut);
            Assert.NotEmpty(sut.Id);
            Assert.Equal(0, sut.Items.Count);
        }

        // TODO: Addtocontainer
        // TODO: Can't Add to container
    }
}
