using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class Arrays
    {
        public static void Demo1()
        {
            // Arrays werden mit ihrer Größe initialisiert
            int[] a = new int[5];
            // über einen Indexer kann auf Elemente zugegriffen werden
            a[0] = 1;
            a[2] = 3;
            // der Index ist 0-basiert
            var third = a[2];

            // array initialization expression
            int[] b = { 5, 4, 3, 2, 1 };
        }

        public static void Demo2()
        {
            // Multidimensionale Arrays

            var rectangular = new int[3, 3]
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 }
            };
            var valueFirstRowSecondColumn = rectangular[0, 1]; // 1

            var jagged = new int[3][]
            {
                new []{1,2,3},
                new []{4,5},
                new []{6}
            };

            var val = jagged[1][1]; // 5
        }
    }
}
