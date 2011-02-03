using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Common;
using NUnit.Framework;
using Conflux.Core;

namespace Conflux.Playground.SampleKernels.Matmul.Tests
{
    [TestFixture]
    public class MatMulKernel_FastTests : MatMulTestsBoilerplate
    {
        protected override float[,] KernelMul(BaseConfig cfg, float[,] a, float[,] b)
        {
            var sw = new Stopwatch();
            try
            {
                sw.Start();
                var result = cfg.Configure<MatMulKernel_Fast>().Execute(a, b);
                Log.WriteLine("Kernel has completed in " + sw.Elapsed);
                return result;
            }
            catch (Exception)
            {
                Log.WriteLine("Kernel has crashed in " + sw.Elapsed);
                throw;
            }
        }
    }
}