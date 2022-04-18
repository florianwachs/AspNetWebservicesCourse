// Mit using werden Namespaces importiert
// Mit using können für Aliase für Typen erzeugt werden
using MathOp = System.Func<decimal, decimal, decimal>;
// Mit using static können Methoden aus einem Typen direkt importiert werden
using static System.Math;
using static System.Console;
using Xunit;

namespace Advanced;

public class Using
{
    [Fact]
    public void DemoTypeAlias()
    {
        MathOp op = (x, y) => x + y;
    }

    [Fact]
    public void UsingNormalMath() => WriteLine(Math.Abs(4.32) * PI);

    [Fact]
    public void UsingStaticMath() => WriteLine(Abs(4.32) * PI);


    [Fact]
    public void DemoDispose()
    {
        using (var res = new MyResource())
        {
            WriteLine("Verwende MyResource");
        }

        // Gleichbedeutend zu:
        MyResource res2 = new MyResource();
        try
        {
            WriteLine("Verwende MyResource");
        }
        finally
        {
            if (res2 != null)
            {
                res2.Dispose();
            }
        }
    }

    public class MyResource : IDisposable
    {
        private bool disposed;

        public MyResource()
        {
            WriteLine("MyResource erzeugt");
        }

        public void DoStuff()
        {
            // Framework Guideline
            EnsureNotDisposed();
        }
        public void Dispose()
        {
            // Framework Guideline:
            // Dispose kann mehrfach aufgerufen werden
            if (!disposed)
            {
                WriteLine("MyResource Aufräumarbeiten");
                disposed = true;
            }
        }

        public void EnsureNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("MyResource");
            }
        }
    }

}
