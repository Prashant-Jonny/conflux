using System.Diagnostics;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Common;

namespace Conflux.Runtime.Cuda
{
    [DebuggerNonUserCode]
    internal class CudaRuntimeJit : BaseRuntimeJit<CudaRuntime, CudaConfig>
    {
        public CudaRuntimeJit(CudaRuntime runtime)
            : base(runtime)
        {
        }
    }
}