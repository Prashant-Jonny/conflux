using System.Runtime.CompilerServices;
using Conflux.Core.Api;
using Libcuda.DataTypes;

namespace Conflux.Core.Kernels
{
    public abstract partial class KernelApi : IGridApi
    {
        dim3 IGridApi.GridDim { get { return GridDim; } }
        extern protected virtual dim3 GridDim { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        dim3 IGridApi.BlockDim { get { return BlockDim; } }
        extern protected virtual dim3 BlockDim { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        int3 IGridApi.BlockIdx { get { return BlockIdx; } }
        extern protected virtual int3 BlockIdx { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        int3 IGridApi.ThreadIdx { get { return ThreadIdx; } }
        extern protected virtual int3 ThreadIdx { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}