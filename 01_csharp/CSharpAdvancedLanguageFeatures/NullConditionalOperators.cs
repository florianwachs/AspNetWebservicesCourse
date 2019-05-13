using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    
    public static class NullConditionalOperators
    {
        public static void BeforeCSharp6()
        {             
            var s = GetStudent(123);

            // Alle Null-Checks durchführen bevor wir auf Count zugreifen können
            int addressCount = s != null && s.Contact != null && s.Contact.Addresses != null ? s.Contact.Addresses.Count : 0;
        }

        public static void WithCSharp6()
        {
            var s = GetStudent(123);

            // Code hinter "?" wird nur ausgeführt wenn davor ein Non-Null Wert ermittelt wurde
            int addressCount = s?.Contact?.Addresses?.Count ?? 0;
        }

        
        private static Student GetStudent(int id)
        {
            return new Student();
        }
    }
    
}
