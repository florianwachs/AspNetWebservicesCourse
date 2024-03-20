using Xunit;

namespace Basics;

// Definition eines Delegate-Types
public delegate decimal MathOperation(decimal x, decimal y);

public class DelegatesAndLambdas
{
    [Fact]
    public void Demo1()
    {
        // Dem Delegate kann jede Methode zugewiesen werden,
        // solange sie der Methodensignatur folgt, die der
        // Delegate vorgibt
        MathOperation op = new MathOperation(Add);
        // oder
        op = Add;

        Console.WriteLine(op(2, 4)); // 6

        op = Multiply;
        Console.WriteLine(op(2, 4)); // 8
    }

    [Fact]
    public void Demo2()
    {
        // Generic Delegates
        // 2 decimal Parameter, decimal Rückgabewert
        Func<decimal, decimal, decimal> op = Add;
        Console.WriteLine(op(2, 4));
    }

    [Fact]
    public void Demo3()
    {
        MathOperation op = (x, y) => x + y;
        Console.WriteLine(op(2, 4)); // 6

        // oder mit Typ Angabe
        MathOperation op2 = (x, y) => x + y;

        // mit Generics
        Func<decimal, decimal, decimal> op3 = (x, y) => x + y;
    }

    [Fact]
    public void Demo4()
    {
        var a = new List<int> { 1, 2, 3, 5, 67, 345, 223334 };

        var biggerThan5 = a.FindAll(n => n > 5);

        // ohne Lambda
        var biggerThan5OhneLambda = a.FindAll(BiggerThan5);
    }

    private bool BiggerThan5(int n)
    {
        return n > 5;
    }

    public decimal Add(decimal x, decimal y)
    {
        return x + y;
    }

    public decimal Multiply(decimal x, decimal y)
    {
        return x * y;
    }
}
