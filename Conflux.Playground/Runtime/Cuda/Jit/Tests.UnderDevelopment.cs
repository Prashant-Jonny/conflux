using NUnit.Framework;
using Conflux.Playground.SampleKernels.Matmul;
using Conflux.Playground.SampleKernels.Ricker;

namespace Conflux.Playground.Runtime.Cuda.Jit
{
    [TestFixture, Category("Under Development")]
    public class UnderDevelopment : Tests
    {
        [Test, Category("Hot")]
        public void MatMulKernel()
        {
            TestKernelCrosscompilation(typeof(MatMulKernel));
        }

        [Test]
        public void MatMulKernel_Fast()
        {
            TestKernelCrosscompilation(typeof(MatMulKernel_Fast));
        }

        [Test]
        public void WaveKernel()
        {
            TestKernelCrosscompilation(typeof(WaveKernel));
        }
    }
}
