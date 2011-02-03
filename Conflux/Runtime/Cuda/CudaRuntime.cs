using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Cuda;
using Conflux.Core.Kernels;
using Conflux.Runtime.Common;
using Conflux.Runtime.Common.Registry;
using Conflux.Runtime.Cuda.Jit;
using Libcuda;
using XenoGears.Assertions;

namespace Conflux.Runtime.Cuda
{
    // todo. review the entire algorithm in order to find potential fail points
    // (including those in XenoGears in general and in IL parser in particular)
    // then create an exception hierarchy and implement decent error handling and reporting

    [Runtime(Platform.Cuda)]
    internal class CudaRuntime : BaseRuntime<CudaConfig, CudaRuntimeJit>
    {
        [DebuggerNonUserCode] public CudaRuntime(CudaConfig config, Type t_kernel) : base(config, t_kernel) { }
        [DebuggerNonUserCode] public static CudaRuntime Current { get { return Runtimes.Active as CudaRuntime; } }

        protected override void CoreRunKernel(IGrid grid, IKernel kernel)
        {
            (Config.Target <= CudaVersions.HardwareIsa).AssertTrue();
            (Config.Version <= CudaVersions.SoftwareIsa).AssertTrue();

            var t_kernel = kernel.GetType().BaseType.BaseType;
            var jitted_kernel = JitCompiler.DoCompile(Config, t_kernel);
            jitted_kernel.Run(grid.GridDim, grid.BlockDim, kernel);
        }
    }
}