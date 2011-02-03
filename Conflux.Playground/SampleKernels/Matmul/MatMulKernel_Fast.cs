using Conflux.Core.Configuration;
using Conflux.Core.Kernels;
using Conflux.Core.Annotations;
using Conflux.Core.Annotations.Semantics;
using Libcuda.DataTypes;
using Conflux.Playground.SampleKernels.Matmul.Domain;
using XenoGears.Functional;
using XenoGears.Assertions;

namespace Conflux.Playground.SampleKernels.Matmul
{
    // note. the algorithm works as follows:
    // 1. The resulting matrix is divided into square blocks that correspond to square thread blocks.
    //    If dimensions ain't exact multiples of block dimensions it's not a problem.
    // 2. Each block calculates corresponding block of the resulting matrix as follows:
    // 3. Horizontal strip of matrix A and vertical strip of matrix B that correspond to the block
    //    get subdivided into chunks of size same or smaller than a square block.
    //    The number of chunks for both strips is the same since A.Dim(1) == B.Dim(0).
    // 4. Corresponding chunks from A and B get loaded in a loop into a shared region of memory.
    //    Each thread in a block loads two values: a single cell from A's chunk and same for B.
    // 5. Multiplying those chunks will produce a partial product of size BlockDim.X * BlockDim.Y
    //    (size might be smaller if size of matrices being multiplied is not a multiple of BlockDim)
    //    n0te. this implies that BlockDim is a square matrix, i.e. BlockDim.X == BlockDim.Y.
    // 6. Accumulating partial products for all suitable chunks will produce a final answer
    // 7. Then each thread writes a single cell of a result (or does nothing if it's a borderline case)

    public abstract class MatMulKernel_Fast : Kernel<float[,], float[,], float[,]>
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
            var minDim = Min(Min(_a.Height(), defaultBlockSize), Min(_b.Width(), defaultBlockSize));
            var blockDim = gridCfg.BlockDim ?? new dim3(minDim, minDim);
            var gridDim = gridCfg.GridDim ?? new dim3((int)Ceilf(1f * _b.Width() / blockDim.X), (int)Ceilf(1f * _a.Height() / blockDim.Y));
            gridCfg.SetDims(gridDim, blockDim);
        }

        protected float[,] _a;
        protected float[,] _b;
        [Result] protected float[,] _c;

        protected override void RunKernel()
        {
            // this is required because partial matrices asub and bsub need to be multiplied
            // i.e. [BlockDim.X * BlockDim.Y] needs to be multipliable by [BlockDim.X, BlockDim.Y]
            (BlockDim.X == BlockDim.Y).AssertTrue();

            // the accumulator - stores a single cell of the resulting matrix
            var c_value = 0f;

            // we need to multiply horizontal and vertical strips that are assigned to our thread block
            // we tile the strips with square blocks and for every corresponding pair of tiles we do the following
            // 1) load them into shared memory, 2) multiply them, 3) update the accumulator
            // once all pairs are processed the accumulator represents a single cell of the result
            for (var i = 0; i < (int)Ceilf(1f * _a.Width() / BlockDim.X); ++i) 
            {
                // Load the chunk from matrix A into shared memory
                var asub = Submatrix(_a, BlockIdx.Y, i);
                var asub_shared = new float[BlockDim.X, BlockDim.X];
                var idle_a = asub.Height <= ThreadIdx.Y || asub.Width <= ThreadIdx.X;
                if (idle_a) asub_shared[ThreadIdx.Y, ThreadIdx.X] = 0;
                else asub_shared[ThreadIdx.Y, ThreadIdx.X] = asub[ThreadIdx.Y, ThreadIdx.X];
                Hints.Sharing.Local(asub_shared);

                // Load the chunk from matrix B into shared memory
                var bsub = Submatrix(_b, i, BlockIdx.X);
                var bsub_shared = new float[BlockDim.X, BlockDim.X];
                var idle_b = bsub.Height <= ThreadIdx.Y || bsub.Width <= ThreadIdx.X;
                if (idle_b) bsub_shared[ThreadIdx.Y, ThreadIdx.X] = 0;
                else bsub_shared[ThreadIdx.Y, ThreadIdx.X] = bsub[ThreadIdx.Y, ThreadIdx.X];
                Hints.Sharing.Local(bsub_shared);

                // ensure that everyone has completed copying data
                // so that other threads can rely on the entire submatrix been loaded
                SyncThreads();

                // compute a part of the single cell of the product
                var stripLen = Min(_a.Width() - i * BlockDim.X, BlockDim.X);
                for (var j = 0; j < stripLen; ++j)
                {
                    // some threads will here process data that's out of range of A or B
                    // however, it's not a problem since such data is initialized by zeros
                    // and it won't interfere with any c_values being accumulated
                    c_value += asub_shared[ThreadIdx.Y, j] * bsub_shared[j, ThreadIdx.X];
                }

                // ensure that everyone has finished with crunching
                // so that we're ready to load subsequent chunks
                SyncThreads();
            }

            // Write Csub to device memory
            // Each thread writes one element
            var csub = Submatrix(_c, BlockIdx.Y, BlockIdx.X);
            if (csub.Height > ThreadIdx.Y && csub.Width > ThreadIdx.X) 
                csub[ThreadIdx.Y, ThreadIdx.X] = c_value;
        }

        protected SubMatrix<float> Submatrix(Matrix<float> m, int blockRow, int blockCol)
        {
            var top = blockRow * BlockDim.Y;
            var left = blockCol * BlockDim.X;
            var height = Min(BlockDim.Y, m.Height - top);
            var width = Min(BlockDim.X, m.Width - left);
            return new SubMatrix<float>(m, top, left, height, width);
        }
    }
}