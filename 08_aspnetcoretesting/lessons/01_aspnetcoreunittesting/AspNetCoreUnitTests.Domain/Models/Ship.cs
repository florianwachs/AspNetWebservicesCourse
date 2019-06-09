using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public class Ship
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public Kg CurrentWeight { get; private set; }
        public Kg MaximumWeight { get; private set; }
        public IReadOnlyCollection<Container> Containers { get; private set; }

        private Ship()
        {
            // For EF Core to hydrate entity from database
            CurrentWeight = Kg.Zero;
            MaximumWeight = Kg.Zero;
            Containers = new List<Container>();
        }
    }
}
