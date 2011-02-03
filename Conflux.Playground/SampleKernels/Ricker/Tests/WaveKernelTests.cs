using Conflux.Core;
using Conflux.Core.Configuration.Cpu;
using Conflux.Core.Configuration.Cuda;
using NUnit.Framework;
using Conflux.Playground.SampleKernels.Matmul.Domain;
using Conflux.Playground.SampleKernels.Ricker.Domain;

namespace Conflux.Playground.SampleKernels.Ricker.Tests
{
    [TestFixture]
    public class WaveKernelTests
    {
        [Test]
        public void Wave_Cpu_1x()
        {
            var kernel = new CpuConfig{Cores = 1}.Configure<WaveKernel>();
            var empty = new Matrix<Cell>(202, 202);
            var after2s = kernel.Execute(empty);
            // todo. validate the result
        }

        [Test]
        public void Wave_Cpu_2x()
        {
            var kernel = new CpuConfig{Cores = 2}.Configure<WaveKernel>();
            var empty = new Matrix<Cell>(202, 202);
            var after2s = kernel.Execute(empty);
            // todo. validate the result
        }

        [Test, Category("Hot")]
        public void WaveCuda()
        {
            var cfg = new CudaConfig();
            cfg.Codebase.OptIn(t => t.Assembly.GetName().Name == "Conflux.Playground");
            var kernel = cfg.Configure<WaveKernel>();
            var empty = new Matrix<Cell>(202, 202);
            var after2s = kernel.Execute(empty);
            // todo. validate the result
        }
    }
}
