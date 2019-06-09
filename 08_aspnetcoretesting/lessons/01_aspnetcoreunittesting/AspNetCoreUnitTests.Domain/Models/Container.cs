using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreUnitTests.Domain.Exceptions;

namespace AspNetCoreUnitTests.Domain.Models
{
    public abstract class Container
    {
        public string Id { get; protected set; }
        public Kg CurrentWeight { get; protected set; }
        public Kg MaximumWeight { get; protected set; }
        public IReadOnlyCollection<ContainerItem> Items { get; protected set; }

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

        public virtual void Add(ContainerItem item)
        {
            if (Contains(item))
            {
                throw new ItemAlreadyInContainerException(item.Id);
            }

            if (!IsWithinWeightLimit(item))
            {
                throw new ContainerOverweightException(Id);
            }

            var newContainerContent = Items.Concat(new[] { item }).ToList();
            Items = newContainerContent;
            CurrentWeight = Kg.Create(newContainerContent.Sum(c => c.Weight.Amount));
        }

        public bool Contains(ContainerItem item) => Items.Any(i => i.Id == item.Id);

        public bool CanAddItem(ContainerItem itemToAdd) => !Contains(itemToAdd) && IsWithinWeightLimit(itemToAdd);

        private bool IsWithinWeightLimit(ContainerItem itemToAdd)
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

        public static EuStandardContainer New() => new EuStandardContainer(Guid.NewGuid().ToString());
    }
}
