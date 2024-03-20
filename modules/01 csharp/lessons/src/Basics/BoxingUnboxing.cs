using Xunit;

namespace Basics;

public class BoxingUnboxing
{
    [Fact]
    public void Demo1()
    {
        int x = 1;
        // Boxing: int => object
        object boxDHL = x;
        object boxUPS = x;

        bool gleichesObjekt = ReferenceEquals(boxDHL, boxUPS);

        // Unboxing object => int
        var y = (int)boxDHL;
        var z = (int)boxUPS;
    }
}
