namespace AspNetCoreUnitTests.Domain.Models
{
    public class ContainerItem
    {
        public string Id { get; private set; }
        public Kg Weight { get; private set; }
        public bool PassedInspection { get; private set; }

        private ContainerItem()
        {
            // For EF Core to hydrate entity from database
        }

        private ContainerItem(string id, Kg weight) : this()
        {
            Id = id;
            Weight = weight;
        }

        public static ContainerItem New(string id, Kg weight) => new ContainerItem(id, weight);
    }
}
