using Xunit;

namespace Basics;

public class Variables
{
    [Fact]
    public void Foo()
    {
        int a = 0;
        double b = 2.3;
        // Das m ist nötig um decimal zu erhalten.
        // Als Standard wird double verwendet.
        decimal c = 3.4m;
        string d = "Hallo";
        List<Tuple<int, string, Dictionary<int, string>>> maybeMakeAClass =
            new List<Tuple<int, string, Dictionary<int, string>>>();
    }

    [Fact]
    public void Var()
    {
        var a = 0;
        var b = 2.3;

        // Das m ist nötig um decimal zu erhalten.
        // Als Standard wird double verwendet.
        var c = 3.4m;
        var d = "Hallo";
        var maybeMakeAClass =
            new List<Tuple<int, string, Dictionary<int, string>>>();
    }
}
