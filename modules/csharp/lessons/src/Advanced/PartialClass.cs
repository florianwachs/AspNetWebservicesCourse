using Xunit;

namespace CSharpAdvancedLanguageFeatures;

public class PartialDemo
{
    [Fact]
    public void Demo1()
    {
        var p = new PartialClass();
        Console.WriteLine(p.LastChanged.HasValue);
        p.GeneratedField = 10;
        Console.WriteLine(p.LastChanged.HasValue);
    }
}

public partial class PartialClass
{
    public DateTime? LastChanged { get; private set; }

    // partial Methode wird implementiert und
    // damit vom Compiler auch berücksichtigt
    partial void Changed(int? oldValue, int? newValue)
    {
        LastChanged = DateTime.Now;
    }
}
