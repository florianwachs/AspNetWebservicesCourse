using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public class NullableTypes
    {
        public static void Demo1()
        {
            int? a = default(int?);
            // oder
            a = null;

            if (a.HasValue)
            {
                // Foo
            }

            int aOrDefault = a.GetValueOrDefault(10);
            // oder
            // null-coalescing operator  
            // https://msdn.microsoft.com/de-de/library/ms173224.aspx
            aOrDefault = a ?? 10;
        }
    }
}
