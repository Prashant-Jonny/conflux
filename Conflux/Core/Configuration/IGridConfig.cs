using Libcuda.DataTypes;

namespace Conflux.Core.Configuration
{
    public interface IGridConfig
    {
        dim3? GridDim { get; set; }
        dim3? BlockDim { get; set; }
        void SetDims(dim3 gridDim, dim3 blockDim);
    }
}