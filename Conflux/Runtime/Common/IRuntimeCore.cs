using Conflux.Core.Kernels;

namespace Conflux.Runtime.Common
{
    internal interface IRuntimeCore
    {
        void CoreRunKernel(IGrid grid, IKernel kernel);
    }
}