using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace CSharpAdvancedLanguageFeatures
{
    public class PatternMatching
    {
        public void IsExpressionWithPatterns(object o)
        {
            if (o is null) return;     // constant pattern "null"
            if (o is int i) // type pattern "int i"
            {
                // Der if-Block wird nur betreten wenn es sich um ein int handelt
                // die Variable i ist nur innerhalb des if-Blocks zugewiesen
                Console.WriteLine(i);
            }

            if (!(o is decimal d))
                return;

            // Ab hier weiß der Compiler das o ein decimal sein muss
            Console.WriteLine(d);
        }

        public void SwitchWithPatterns(Shape shape)
        {
            switch (shape)
            {
                case Circle c:
                    WriteLine($"circle with radius {c.Radius}");
                    break;
                // Es können auch Bedingungen mit when definiert werden
                case Rectangle s when (s.Length == s.Height):
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

        public class Shape
        {
        }
        public class Rectangle : Shape
        {
            public decimal Length { get; set; }
            public decimal Height { get; set; }
        }
        public class Circle : Shape
        {
            public decimal Radius { get; set; }
        }
    }
}
