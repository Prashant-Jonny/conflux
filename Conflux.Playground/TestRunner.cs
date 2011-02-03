using System;
using System.Collections.Generic;

namespace Conflux.Playground
{
    internal class TestRunner
    {
        public static void Main(String[] args)
        {
            // see more details at http://www.nunit.org/index.php?p=consoleCommandLine&r=2.5.5
            var nunitArgs = new List<String>();
            nunitArgs.Add("/run:Conflux.Playground.Demo");
//            nunitArgs.Add("/run:Conflux.Playground.Runtime.Cuda.Jit");
//            nunitArgs.Add("/run:Conflux.Playground.SampleKernels.Matmul.Tests.MatMulKernelTests");
//            nunitArgs.Add("/run:Conflux.Playground.SampleKernels.Matmul.Tests.MatMulKernel_FastTests");
//            nunitArgs.Add("/run:Conflux.Playground.SampleKernels.Ricker.Tests.WaveKernelTests");
            nunitArgs.Add("/include:Hot");
            nunitArgs.Add("/domain:None");
            nunitArgs.Add("/noshadow");
            nunitArgs.Add("/nologo");
            nunitArgs.Add("Conflux.Playground.exe");
            NUnit.ConsoleRunner.Runner.Main(nunitArgs.ToArray());
        }
    }
}
