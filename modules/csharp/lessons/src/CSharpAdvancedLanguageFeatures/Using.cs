using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Mit using werden Namespaces importiert
using System.Threading.Tasks;
// Mit using können für Aliase für Typen erzeugt werden
using MathOp = System.Func<decimal, decimal, decimal>;
// Mit using static können Methoden aus einem Typen direkt importiert werden
using static System.Math;
using static System.Console;

namespace CSharpAdvancedLanguageFeatures
{
    public static class Using
    {
        public static void DemoTypeAlias()
        {
            MathOp op = (x, y) => x + y;
        }

        public static void UsingNormalMath(int value) => Console.WriteLine(Math.Abs(value) * Math.PI);
        public static void UsingStaticMath(int value) => WriteLine(Abs(value) * PI);

        public static void DemoDispose()
        {
            using (var res = new MyResource())
            {
                Console.WriteLine("Verwende MyResource");
            }

            // Gleichbedeutend zu:
            MyResource res2 = new MyResource();
            try
            {
                Console.WriteLine("Verwende MyResource");
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
                Console.WriteLine("MyResource erzeugt");
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
                    Console.WriteLine("MyResource Aufräumarbeiten");
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
}
