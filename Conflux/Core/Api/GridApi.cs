using System.Diagnostics;
using Conflux.Core.Api.Registry;
using Libcuda.DataTypes;
using Conflux.Runtime.Common.Registry;

namespace Conflux.Core.Api
{
    [Api, DebuggerNonUserCode]
    public static class GridApi
    {
        public static dim3 GridDim { get { return Runtimes.ActiveApi.GridDim; } }
        public static dim3 BlockDim { get { return Runtimes.ActiveApi.BlockDim; } }

        public static int3 BlockIdx { get { return Runtimes.ActiveApi.BlockIdx; } }
        public static int3 ThreadIdx { get { return Runtimes.ActiveApi.ThreadIdx; } }
    }
}