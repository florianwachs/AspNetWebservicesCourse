using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public class BoxingUnboxing
    {
        public static void Demo1()
        {
            int x = 1;
            // Boxing: int => object
            object boxDHL = x;
            object boxUPS = x;

            bool gleichesObjekt = object.ReferenceEquals(boxDHL, boxUPS);

            // Unboxing object => int
            var y = (int)boxDHL;
            var z = (int)boxUPS;
        }
    }
}
