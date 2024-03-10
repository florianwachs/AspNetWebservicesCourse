using Xunit;

namespace Basics;

public class NullableTypes
{
    [Fact]
    public void Demo1()
    {
        int? a = default;
        // oder
        a = null;

        if (a.HasValue)
        {
            // Foo
        }

        int aOrDefault = a.GetValueOrDefault(10);
        // oder
        // null-coalescing operator  
        // https://msdn.microsoft.com/de-de/library/ms173224.aspx
        aOrDefault = a ?? 10;
    }
}
