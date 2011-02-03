using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Auto;
using Conflux.Core.Configuration.Cpu;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Common;
using Conflux.Runtime.Common.Registry;
using Conflux.Runtime.Cpu;
using Conflux.Runtime.Cuda;
using Libcuda.Api.Devices;

namespace Conflux.Runtime.Auto
{
    [Runtime(Platform.Auto)]
    [DebuggerNonUserCode]
    internal class AutoRuntime : AbstractRuntime
    {
        private readonly IRuntime _impl;

        public AutoRuntime(AutoConfig config, Type t_kernel)
            : base(config, t_kernel)
        {
            var cudaDevice = CudaDevice.Current;
            if (cudaDevice != null)
            {
                var cfg = CudaConfig.Default;
                _impl = new CudaRuntime(cfg, t_kernel);
            }
            else
            {
                var cfg = CpuConfig.Default;
                _impl = new CpuRuntime(cfg, t_kernel);
            }
        }

        public override Object Execute(params Object[] args)
        {
            return _impl.Execute(args);
        }
    }
}