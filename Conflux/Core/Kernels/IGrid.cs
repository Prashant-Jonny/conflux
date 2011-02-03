using System.Diagnostics;
using Conflux.Core.Api;
using Conflux.Core.Configuration;
using Libcuda.DataTypes;
using XenoGears.Assertions;

namespace Conflux.Core.Kernels
{
    public interface IGrid
    {
        dim3 GridDim { get; }
        dim3 BlockDim { get; }
    }

    [DebuggerNonUserCode]
    internal static class GridHelpers
    {
        public static IGrid ToGrid(this IGridConfig gridCfg)
        {
            return new Grid(gridCfg.GridDim.AssertValue(), gridCfg.BlockDim.AssertValue());
        }

        public static IGrid ToGrid(this IGridApi gridApi)
        {
            return new Grid(gridApi.GridDim, gridApi.BlockDim);
        }

        [DebuggerNonUserCode]
        private class Grid : IGrid
        {
            public dim3 GridDim { get; private set; }
            public dim3 BlockDim { get; private set; }

            public Grid(dim3 gridDim, dim3 blockDim)
            {
                GridDim = gridDim;
                BlockDim = blockDim;
            }
        }
    }
}