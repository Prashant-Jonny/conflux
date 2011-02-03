using Conflux.Core.Configuration.Cpu;
using Conflux.Core.Configuration.Cuda;
using NUnit.Framework;
using XenoGears.Functional;

namespace Conflux.Playground.SampleKernels.Matmul.Tests
{
    [TestFixture]
    public abstract class MatMulTestsBoilerplate : MatMulTestsCore
    {
        [Test]
        public void SmallTest_Cpu_1x()
        {
            var cfg = new CpuConfig{Cores = 1};
            1.TimesDo(() => SmallTest(cfg));
        }

        [Test]
        public void SmallTest_Cpu_2x()
        {
            var cfg = new CpuConfig{Cores = 2};
            1.TimesDo(() => SmallTest(cfg));
        }

        [Test, Category("Hot")]
        public void SmallTest_Cuda()
        {
            var cfg = new CudaConfig();
            cfg.Codebase.OptIn(t => t.Assembly.GetName().Name == "Conflux.Playground");
            1.TimesDo(() => SmallTest(cfg));
        }

        [Test]
        public void MediumTest_Cpu_1x()
        {
            var cfg = new CpuConfig{Cores = 1};
            1.TimesDo(() => MediumTest(cfg));
        }

        [Test]
        public void MediumTest_Cpu_2x()
        {
            var cfg = new CpuConfig{Cores = 2};
            1.TimesDo(() => MediumTest(cfg));
        }

        [Test]
        public void MediumTest_Cuda()
        {
            var cfg = new CudaConfig();
            cfg.Codebase.OptIn(t => t.Assembly.GetName().Name == "Conflux.Playground");
            1.TimesDo(() => MediumTest(cfg));
        }

        [Test]
        public void BigTest_Cpu_1x()
        {
            var cfg = new CpuConfig{Cores = 1};
            1.TimesDo(() => BigTest(cfg));
        }

        [Test]
        public void BigTest_Cpu_2x()
        {
            var cfg = new CpuConfig{Cores = 2};
            1.TimesDo(() => BigTest(cfg));
        }

        [Test]
        public void BigTest_Cuda()
        {
            var cfg = new CudaConfig();
            cfg.Codebase.OptIn(t => t.Assembly.GetName().Name == "Conflux.Playground");
            1.TimesDo(() => BigTest(cfg));
        }
    }
}