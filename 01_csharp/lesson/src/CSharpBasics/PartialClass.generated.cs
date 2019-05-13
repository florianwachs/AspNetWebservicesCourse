using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    partial class PartialClass
    {
        private int? generatedField;
        public int? GeneratedField
        {
            get
            {
                Getting(ref generatedField);
                return generatedField;
            }
            set
            {
                var old = generatedField;
                if (old != value)
                {
                    generatedField = value;
                    Changed(old, value);
                }
            }
        }
        // Wenn die Methoden nicht implementiert werden
        // entfernt der Compiler die Aufrufe
        partial void Changed(int? oldValue, int? newValue);
        partial void Getting(ref int? value);

    }
}
