using Xunit;

namespace Advanced;

public class ExtensionMethods
{
    [Fact]
    public void Demo1()
    {
        var p = new Person("Franzi", "Maier");
        // Für den Aufrufer sieht es so aus,
        // als wäre GetFullName() teil der Klassendefinition
        Console.WriteLine(p.GetFullName());
        // in Wirklichkeit wird aber nur folgendes aufgerufen
        Console.WriteLine(p.GetFullName());
    }
}

// Extension Methods müssen in einer
// static class definiert sein
// damit sie auf den "erweiterten" Typen angewendet werden
// können, muss diese Klasse entweder im gleichen Namepace
// liegen oder der Namespace per using importiert werden.
public static class PersonExtensions
{
    // mit this wird
    // Extension-Methods sind normale statische Methoden,
    // können also auch weitere Parameter enthalten
    public static string GetFullName(this Person person, string prefix = null)
    {
        // Extension-Methods sind syntactic sugar.
        // Sie erweitern nicht die Klassendefinition,
        // sie können nur auf die public-Member zugreifen
        return prefix + person.FirstName + " " + person.LastName;
    }
}

