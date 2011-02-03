using System;
using NUnit.Framework;
using XenoGears.Functional;

namespace Conflux.Playground.Demo
{
    [TestFixture]
    public partial class Tests
    {
        [Test, Category("Hot")]
        public void MatMul()
        {
            var a = new float[2, 3]
            {
                {1, 1, 1},
                {1, 1, 1}
            };

            var b = new float[3, 2]
            {
                {1, 1},
                {1, 1},
                {1, 1}
            };

            Console.WriteLine();
            Console.WriteLine();
            var c = Mul(a, b);

            Print("a", a);
            Print("b", b);
            Print("c", c);
        }

        private static void Print<T>(String name, T[,] matrix)
        {
            Console.WriteLine();
            Console.WriteLine(name + ":");
            for (var i = 0; i < matrix.Height(); ++i)
            {
                for (var j = 0; j < matrix.Width(); ++j)
                {
                    Console.Write(matrix[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
