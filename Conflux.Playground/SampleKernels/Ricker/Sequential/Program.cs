using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Conflux.Playground.SampleKernels.Ricker.Sequential
{
    [TestFixture]
    public class Program
    {
        [Test]
        public void Test()
        {
            var sw = Stopwatch.StartNew();
            new Calculations().DoCalculate();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
