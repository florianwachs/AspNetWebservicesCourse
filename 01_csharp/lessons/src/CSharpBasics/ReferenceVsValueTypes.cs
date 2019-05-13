using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public interface ICanMove
    {
        void MoveBy(int x, int y);
    }

    public struct PointStruct : ICanMove
    {
        public int X;
        public int Y;

        public PointStruct(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void MoveBy(int x, int y)
        {
            X += x;
            Y += y;
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}", X, Y);
        }
    }

    public class PointClass
    {
        public int X;
        public int Y;

        public PointClass(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}", X, Y);
        }
    }

    public static class ReferenceVsValueTypes
    {
        public static void DemoClass()
        {
            var p1 = new PointClass(5, 5);
            var p2 = p1;
            p2.X = 10;
            p2.Y = 10;

            Console.WriteLine("p1: " + p1.ToString());
            Console.WriteLine("p2: " + p2.ToString());
        }

        public static void DemoStruct()
        {
            var p1 = new PointStruct(5, 5);
            var p2 = p1;
            p2.X = 10;
            p2.Y = 10;

            Console.WriteLine("p1: " + p1.ToString());
            Console.WriteLine("p2: " + p2.ToString());
        }
    }
}
