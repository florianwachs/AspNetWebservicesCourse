using Xunit;

namespace Advanced;

public class LocalFunctions
{
    [Fact]
    public void Demo1()
    {
        Console.WriteLine(Fibonacci(2));
    }
    private int Fibonacci(int x)
    {
        if (x < 0) throw new ArgumentException("Less negativity please!", nameof(x));
        return Fib(x).current; // Hier endet die eigentliche Funktion

        // Lokale Funktion mit ValueTuple als return Type
        // Kann auch auf lokale Variablen der umgebenden Funktion zugreifen
        (int current, int previous) Fib(int i)
        {
            if (i == 0) return (1, 0);
            var (p, pp) = Fib(i - 1);
            return (p + pp, p);
        }
    }
}
