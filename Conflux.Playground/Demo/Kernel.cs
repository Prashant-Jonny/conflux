using Conflux.Core;
using Conflux.Core.Annotations.Semantics;
using Conflux.Core.Configuration;
using Conflux.Core.Configuration.Cpu;
using Conflux.Core.Configuration.Cuda;
using Conflux.Core.Kernels;
using Libcuda.DataTypes;
using NUnit.Framework;
using XenoGears.Assertions;
using XenoGears.Functional;

namespace Conflux.Playground.Demo
{
    [TestFixture]
    public partial class Tests
    {
        private float[,] Mul(float[,] a, float[,] b)
        {
            var cfg = new CudaConfig();
//            var cfg = new CpuConfig(){Cores = 1};
            var kernel = cfg.Configure<MatMulKernel>();
            return kernel.Execute(a, b);
        }

        public abstract class MatMulKernel : Kernel<float[,], float[,], float[,]>
        {
            protected float[,] _a;
            protected float[,] _b;
            [Result] protected float[,] _c;

            protected override void Initialize(IGridConfig gridCfg, float[,] a, float[,] b)
            {
                _a = a;
                _b = b;

                (_a.Width() == _b.Height()).AssertTrue();
                _c = new float[a.Height(), b.Width()];

                var blockDim = new dim3(16, 16, 1);
                var gridDim = new dim3((int)Math.Ceil(1.0 * _b.Width() / 16), (int)Math.Ceil(1.0 * _a.Height() / 16), 1);
                gridCfg.SetDims(gridDim, blockDim);
            }

            protected override void RunKernel()
            {
                var row = BlockIdx.Y * BlockDim.Y + ThreadIdx.Y;
                var col = BlockIdx.X * BlockDim.X + ThreadIdx.X;
                if (row >= _a.Height() || col >= _b.Width()) return;

                var acc = 0f;
                for (var i = 0; i < _a.Width(); ++i)
                {
                    acc += _a[row, i] * _b[i, col];
                }

                _c[row, col] = acc;
            }
        }
    }
}
