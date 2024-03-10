using Xunit;

namespace Advanced;

public class StringInterpolation
{
    [Fact]
    public void WithoutInterpolation()
    {
        var p = GetPerson();
        // Verwendung von numerierten Platzhaltern im String
        var text = string.Format("Der User {0} {1} wurde am {2} erzeugt.", p.FirstName, p.LastName, p.Created);
        Console.WriteLine(text);
    }

    [Fact]
    public void WithInterpolation()
    {
        var p = GetPerson();

        // Direkte Verwendung von Variablen im String. "$" ermöglicht die Interpolation
        var text = $"Der User {p.FirstName} {p.LastName} wurde am {p.Created} erzeugt.";
        Console.WriteLine(text);
    }

    private static Person GetPerson() => new Person { FirstName = "Chuck", LastName = "Norris" };
}
