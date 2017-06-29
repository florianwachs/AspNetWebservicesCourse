using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pricing.API.Models
{
    public class PriceResponse
    {
        public IEnumerable<Price> Prices { get; set; }
    }
}
