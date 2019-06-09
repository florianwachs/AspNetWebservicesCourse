using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public abstract class Container
    {
        public string Id { get; protected set; }
        public Kg CurrentWeight { get; protected set; }
        public Kg MaximumWeight { get; protected set; }
        public IReadOnlyCollection<ContainerItem> Items { get; private set; }

        private Container()
        {
            // For EF Core to hydrate entity from database
            Items = new List<ContainerItem>();
            CurrentWeight = Kg.Zero;
        }

        protected Container(string id, Kg maximumWeight) : this()
        {
            Id = id;
            MaximumWeight = maximumWeight;
        }


        public bool CanAddItem(ContainerItem itemToAdd)
        {
            return (itemToAdd.Weight + CurrentWeight) < MaximumWeight;
        }
    }

    public class EuStandardContainer : Container
    {

        private static readonly Kg MaximumWeightForEU = Kg.Create(10000);

        private EuStandardContainer(string id) : base(id, MaximumWeightForEU)
        {
        }

        public static EuStandardContainer New()
        {
            return new EuStandardContainer(Guid.NewGuid().ToString());
        }
    }
}
