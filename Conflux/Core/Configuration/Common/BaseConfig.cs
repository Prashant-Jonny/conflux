using System.Diagnostics;
using Libcuda.DataTypes;

namespace Conflux.Core.Configuration.Common
{
    [DebuggerNonUserCode]
    public abstract class BaseConfig : AbstractConfig, IGridConfig
    {
        // Dimensions of computational grid and of its subnodes - blocks.
        //
        // Kernel might override these settings with ones it considers to be optimal.
        // This can be done during the initialization (the Initialize method).

        public dim3? GridDim { get; set; }
        public dim3? BlockDim { get; set; }
        public void SetDims(dim3 gridDim, dim3 blockDim)
        {
            GridDim = gridDim;
            BlockDim = blockDim;
        }

        protected BaseConfig()
        {
            // dimensions are always reset
            // if config is created from default prototype

            GridDim = null;
            BlockDim = null;
        }

        protected BaseConfig(AbstractConfig proto)
            : base(proto)
        {
        }
    }
}