using System;
using Conflux.Core.Configuration;
using Conflux.Core.Kernels;
using Conflux.Core.Annotations.Semantics;
using Libcuda.DataTypes;
using XenoGears.Functional;
using XenoGears.Assertions;

namespace Conflux.Playground.SampleKernels.Matmul
{
    public abstract class MatMulKernel : Kernel<float[,], float[,], float[,]>
    {
        protected override void Initialize(IGridConfig gridCfg, float[,] a, float[,] b)
        {
            (a.Width() == b.Height()).AssertTrue();

            _a = a;
            _b = b;
            _c = new float[_a.Height(), _b.Width()];

            if (gridCfg.GridDim != null && gridCfg.BlockDim != null) return;
            (gridCfg.GridDim == null ^ gridCfg.BlockDim == null).AssertFalse();

            const int defaultBlockSize = 16;
            var blockDim = gridCfg.BlockDim ?? new dim3(Min(_b.Width(), defaultBlockSize), Min(_a.Height(), defaultBlockSize));
            var gridDim = gridCfg.GridDim ?? new dim3((int)Ceilf(1f * _b.Width() / blockDim.X), (int)Ceilf(1f * _a.Height() / blockDim.Y));
            gridCfg.SetDims(gridDim, blockDim);
        }

        protected float[,] _a;
        protected float[,] _b;
        [Result] protected float[,] _c;

        protected override void RunKernel()
        {
            var row = BlockIdx.Y * BlockDim.Y + ThreadIdx.Y;
            var col = BlockIdx.X * BlockDim.X + ThreadIdx.X;
            // this is necessary in case when matrix dims ain't multiples of block dims
            if (_a.Height() <= row || _b.Width() <= col) return;

            var c_value = 0f;
            for (var dim = 0; dim < _a.Width(); ++dim)
            {
                c_value += _a[row, dim] * _b[dim, col];
            }

            _c[row, col] = c_value;
        }
    }
}