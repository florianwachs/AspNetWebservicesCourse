using AspNetCoreUnitTests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.NotNull(sut);
            Assert.NotEmpty(sut.Id);
            Assert.Equal(0, sut.Items.Count);
        }

        // TODO: Addtocontainer
        // TODO: Can't Add to container

      
    }
}
