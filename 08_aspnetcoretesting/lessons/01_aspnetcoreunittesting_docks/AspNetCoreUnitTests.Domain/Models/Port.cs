using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public class Port
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<Dock> Docks { get; private set; }

        private Port()
        {
            // For EF Core to hydrate entity from database
            Docks = new List<Dock>();
        }
    }
}
