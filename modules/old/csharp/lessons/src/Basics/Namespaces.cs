// Namespaces müssen nicht mit der Dateisystemstruktur übereinstimmen
// Gibt es in der Datei nur einen Namespace, kann der File-scope namespace verwendet werden.
// Es wäre z.B. namespace CSharpLanguageBasics; (die geschweiften klammern wären dann nicht notwendig und
// man spart sich eine Einrückungsebene)

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

