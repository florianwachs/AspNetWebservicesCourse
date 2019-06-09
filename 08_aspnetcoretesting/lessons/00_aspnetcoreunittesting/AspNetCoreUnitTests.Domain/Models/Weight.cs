using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Models
{
    public enum WeightUnits
    {
        Kg,
        Pounds,
    }

    public class Weight
    {
        public static readonly Weight None = new Weight(0, WeightUnits.Kg);
        public WeightUnits Unit { get; private set; }
        public decimal Amount { get; private set; }

        private Weight()
        {
            // For EF Core to hydrate entity from database
        }

        public Weight(decimal amount, WeightUnits unit)
        {
            Amount = amount;
            Unit = unit;
        }

        public static Weight CreateInKg(decimal amount)
        {
            return new Weight(amount, WeightUnits.Kg);
        }

    }
}
