using Xunit;
using static System.Console;

namespace Advanced;

public class PatternMatching
{
    [Fact]
    public void Demo1()
    {
        IsExpressionWithPatterns(2);
    }

    [Fact]
    public void Demo2()
    {
        SwitchWithPatterns(new Rectangle());
    }

    [Fact]
    public void Demo3()
    {
        SwitchExpressionWithPatters(new Circle() { Radius = 3 });
    }

    public void IsExpressionWithPatterns(object o)
    {
        if (o is null) return;     // constant pattern "null"
        if (o is int i) // type pattern "int i"
        {
            // Der if-Block wird nur betreten wenn es sich um ein int handelt
            // die Variable i ist nur innerhalb des if-Blocks zugewiesen
            WriteLine(i);
        }

        if (!(o is decimal d))
            return;

        // Ab hier weiß der Compiler das o ein decimal sein muss
        WriteLine(d);
    }

    public void SwitchWithPatterns(Shape shape)
    {
        switch (shape)
        {
            case Circle c:
                WriteLine($"circle with radius {c.Radius}");
                break;
            // Es können auch Bedingungen mit when definiert werden
            case Rectangle s when s.Length == s.Height:
                WriteLine($"{s.Length} x {s.Height} square");
                break;
            case Rectangle r:
                WriteLine($"{r.Length} x {r.Height} rectangle");
                break;
            default:
                WriteLine("<unknown shape>");
                break;
            case null:
                throw new ArgumentNullException(nameof(shape));
        }
    }

    public double SwitchExpressionWithPatters(Shape shape)
    {
        return shape switch
        {
            Circle c => Math.Pow(c.Radius, 2) * Math.PI,
            Rectangle r => r.Length * r.Height,
            _ => throw new ArgumentException(nameof(shape)),
        };
    }

    public class Shape
    {
    }
    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Height { get; set; }
    }
    public class Circle : Shape
    {
        public double Radius { get; set; }
    }
}
