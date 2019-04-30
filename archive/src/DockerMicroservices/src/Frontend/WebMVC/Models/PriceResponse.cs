using System.Collections.Generic;

namespace WebMVC.Models
{
    public class PriceResponse
    {
        public IEnumerable<Price> Prices { get; set; }
    }
}
