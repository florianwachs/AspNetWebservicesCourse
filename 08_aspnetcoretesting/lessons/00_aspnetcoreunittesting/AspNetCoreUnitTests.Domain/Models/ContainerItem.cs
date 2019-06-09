using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public class ContainerItem
    {
        public string Id { get; private set; }
        public Weight Weight { get; private set; }
        public bool PassedInspection { get; private set; }

        private ContainerItem()
        {
            // For EF Core to hydrate entity from database
        }
    }
}
