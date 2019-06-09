using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public class Container
    {
        public string Id { get; private set; }
        public Weight CurrentWeight { get; private set; }
        public Weight MaximumWeight { get; private set; }
        public IReadOnlyCollection<ContainerItem> Items { get; private set; }

        private Container()
        {
            // For EF Core to hydrate entity from database
            Items = new List<ContainerItem>();
            CurrentWeight = Weight.None;
            MaximumWeight = Weight.None;
        }
    }
}
