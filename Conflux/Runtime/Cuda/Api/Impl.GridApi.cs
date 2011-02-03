using Conflux.Core.Api;
using Libcuda.DataTypes;

namespace Conflux.Runtime.Cuda.Api
{
    internal abstract partial class Impl : IGridApi
    {
        public dim3 GridDim { get { return Ctm.GridDim; } }
        public dim3 BlockDim { get { return Ctm.BlockDim; } }

        public int3 BlockIdx { get { return Ctm.BlockIdx; } }
        public int3 ThreadIdx { get { return Ctm.ThreadIdx; } }
    }
}
