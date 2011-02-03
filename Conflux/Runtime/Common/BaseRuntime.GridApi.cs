using System;
using Conflux.Core.Api;
using Libcuda.DataTypes;
using XenoGears.Assertions;

namespace Conflux.Runtime.Common
{
    internal abstract partial class BaseRuntime<TConfig, TJit> : IGridApi
    {
        dim3 IGridApi.GridDim { get { return GridDim; } }
        protected virtual dim3 GridDim
        {
            get { return Config.GridDim.AssertValue(); }
        }

        dim3 IGridApi.BlockDim { get { return BlockDim; } }
        protected virtual dim3 BlockDim
        {
            get { return Config.BlockDim.AssertValue(); }
        }

        int3 IGridApi.BlockIdx { get { return BlockIdx; } }
        protected virtual int3 BlockIdx
        {
            get { throw new NotSupportedException("Calls to IGridApi.BlockIdx should have been crosscompiled by the runtime."); }
        }

        int3 IGridApi.ThreadIdx { get { return ThreadIdx; } }
        protected virtual int3 ThreadIdx
        {
            get { throw new NotSupportedException("Calls to IGridApi.ThreadIdx should have been crosscompiled by the runtime."); }
        }
    }
}
