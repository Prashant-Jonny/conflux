using Conflux.Core.Api.Registry;
using Libcuda.DataTypes;

namespace Conflux.Core.Api
{
    [Api]
    public interface IGridApi
    {
        dim3 GridDim { get; }
        dim3 BlockDim { get; }

        int3 BlockIdx { get; }
        int3 ThreadIdx { get; }
    }
}