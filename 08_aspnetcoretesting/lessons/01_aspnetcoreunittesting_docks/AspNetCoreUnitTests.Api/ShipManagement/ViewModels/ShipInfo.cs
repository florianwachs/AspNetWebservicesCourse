using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Api.ShipManagement.ViewModels
{
    public class ShipInfo
    {
        public string Id { get; set; }
        public decimal CurrentLoadInKg { get; set; }
        public decimal MaximumLoadInKg { get; set; }
    }
}
