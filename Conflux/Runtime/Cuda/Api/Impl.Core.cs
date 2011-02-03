using System.Diagnostics;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Api.Registry;
using Libcuda.Api.Devices;

namespace Conflux.Runtime.Cuda.Api
{
    [Impl, DebuggerNonUserCode]
    internal abstract partial class Impl
    {
        protected CudaConfig Cfg { get { return CudaConfig.Current; } }
        protected CudaDevice Device { get { return CudaDevice.Current; } }
    }
}
