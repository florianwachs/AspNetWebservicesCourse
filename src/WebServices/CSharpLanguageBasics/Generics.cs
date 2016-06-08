using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class Generics
    {
        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static void Demo1()
        {
            var myStringStack = new MyAwesomeStack<string>();
            myStringStack.Push("hello");

            var myIntStack = new MyAwesomeStack<int>();
            myIntStack.Push(1);
        }

        public static void Demo2()
        {
            int x = 5;
            int y = 10;

            // man kann die Typenliste angeben
            Swap<int>(ref x, ref y);

            // oder es den Compiler machen lassen
            Swap(ref x, ref y);
        }

        public static void Demo3()
        {
            var studentFactory = new MyFactory<Student>();
            var students = studentFactory.Make(5);
        }
    }

    // T ist ein generischer Typ-Parameter
    // mehrere Typen durch Komma trennen
    public class MyAwesomeStack<T>
    {
        private const int Max_Size = 100;

        private int position;
        private T[] data = new T[Max_Size];

        public void Push(T item)
        {
            data[position++] = item;
        }

        public T Pop()
        {
            return data[position--];
        }

    }

    public class MyFactory<T> where T : new()
    {
        public T[] Make(int count)
        {
            return Enumerable.Range(0, count).Select(_ => new T()).ToArray();
        }
    }


    public class MyGenericClass<T, U>
        where T : Student, ICanMove
        where U : new()
    {
        // T muss von Student ableiten oder Student sein und das ICanMove Interface implementieren
        // U muss einen parameterlosen Konstruktor besitzen
    }
}
