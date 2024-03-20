using Xunit;

namespace Advanced;

public class NullConditionalOperators
{
    [Fact]
    public void BeforeCSharp6()
    {
        var s = GetStudent(123);

        // Alle Null-Checks durchführen bevor wir auf Count zugreifen können
        int addressCount = s != null && s.Contact != null && s.Contact.Addresses != null ? s.Contact.Addresses.Count : 0;
    }

    [Fact]
    public void WithCSharp6AndHigher()
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

