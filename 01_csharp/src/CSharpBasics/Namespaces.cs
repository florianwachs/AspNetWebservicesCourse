using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespaces müssen nicht mit der Dateisystemstruktur übereinstimmen
namespace CSharpLanguageBasics
{
    namespace Service
    {
        // Klassen, Structs, Enums
    }

    namespace Service.Tools
    {
        // Können auch geschachtelt werden
        namespace Nested
        {
            // Klassen, Structs, Enums
        }
    }
}

// Flache namespace-Struktur
namespace CSharpLanguageBasics.Service
{
    // Klassen, Structs, Enums
}

namespace CSharpLanguageBasics.Service.Tools
{
    // Klassen, Structs, Enums
}

namespace CSharpLanguageBasics.Service.Tools.Nested
{
    // Klassen, Structs, Enums
}

