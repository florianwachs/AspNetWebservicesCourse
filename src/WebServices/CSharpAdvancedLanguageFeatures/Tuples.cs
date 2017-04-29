using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    public class Tuples
    {
        // Tuple ist als Reference Type implementiert
        public Tuple<decimal, decimal> GetAmountAndDiscountTuple() => Tuple.Create(100m, 20m);

        // Liefern beide ValueTuples welche als Value Type implementiert sind
        // Für .NET Framework < v4.7 muss das NuGet-Package System.ValueTuple vorhanden sein
        public (decimal, decimal) GetAmountAndDiscountValueTupleNoName() => (100m, 20m);
        public (decimal amount, decimal discountInPercent) GetAmountAndDiscountValueTupleWithName()
        {
            return (100m, 20m);
        }

        public void UseTuples()
        {
            var t1 = GetAmountAndDiscountTuple();
            Console.WriteLine($"Amount {t1.Item1} with Discount {t1.Item2}");

            var t2 = GetAmountAndDiscountValueTupleNoName();
            Console.WriteLine($"Amount {t2.Item1} with Discount {t2.Item2}");

            var t3 = GetAmountAndDiscountValueTupleWithName();
            Console.WriteLine($"Amount {t3.amount} with Discount {t3.discountInPercent}");
        }
    }
}
