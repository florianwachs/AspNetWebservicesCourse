using AspNetCoreUnitTests.Domain.Exceptions;
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

        [Fact]
        public void Should_be_able_to_add_items()
        {
            // Arrange
            var item1 = (id: "tofu", weight: Kg.Create(200));
            var item2 = (id: "Adiletten", weight: Kg.Create(400));

            // Act
            var sut = EuStandardContainer.New();
            var containerItem1 = ContainerItem.New(item1.id, item1.weight);
            var containerItem2 = ContainerItem.New(item2.id, item2.weight);

            sut.Add(containerItem1);
            sut.Add(containerItem2);

            // Assert
            Assert.Equal(2, sut.Items.Count);
        }

        [Fact]
        public void Should_modifiy_currentweight_if_items_added()
        {
            // Arrange
            var item1 = (id: "tofu", weight: Kg.Create(200));
            var item2 = (id: "Adiletten", weight: Kg.Create(400));

            // Act
            var sut = EuStandardContainer.New();
            var containerItem1 = ContainerItem.New(item1.id, item1.weight);
            var containerItem2 = ContainerItem.New(item2.id, item2.weight);

            sut.Add(containerItem1);
            sut.Add(containerItem2);

            // Assert
            var expectedWeight = item1.weight + item2.weight;
            Assert.Equal(expectedWeight, sut.CurrentWeight);
        }

        [Fact]
        public void Should_not_be_able_to_add_item_if_maximum_weight_is_reached()
        {
            var sut = EuStandardContainer.New();

            var containerItem1 = ContainerItem.New("tofu", sut.MaximumWeight + Kg.Create(1));

            Assert.False(sut.CanAddItem(containerItem1));
            Assert.Throws<ContainerOverweightException>(() => sut.Add(containerItem1));
        }
    }
}
