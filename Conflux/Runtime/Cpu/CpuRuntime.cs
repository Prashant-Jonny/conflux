using System;
using System.Diagnostics;
using System.Threading;
using Conflux.Core.Configuration.Cpu;
using Conflux.Runtime.Cpu.Jit;
using Libcuda.DataTypes;
using Conflux.Core.Kernels;
using Conflux.Runtime.Common;
using Conflux.Runtime.Common.Registry;
using XenoGears.Functional;
using XenoGears.Assertions;
using System.Linq;

namespace Conflux.Runtime.Cpu
{
    // todo. review the entire algorithm in order to find potential fail points
    // (including those in XenoGears in general and in IL parser in particular)
    // then create an exception hierarchy and implement decent error handling and reporting

    [Runtime(Platform.Cpu)]
    internal class CpuRuntime : BaseRuntime<CpuConfig, CpuRuntimeJit>
    {
        [DebuggerNonUserCode] public CpuRuntime(CpuConfig config, Type t_kernel) : base(config, t_kernel) { }
        [DebuggerNonUserCode] public static CpuRuntime Current { get { return Runtimes.Active as CpuRuntime; } }

        protected override void CoreRunKernel(IGrid grid, IKernel kernel)
        {
            // todo. so far we have just this totally uninformative flag
            // 1) this solution is better than receiving AppDomain.UnhandledException
            //    since this way we ensure that we track only threads that we create
            // 2) further implementations might include exception unwrapping and multiplexing
            //    just like TPL/PLINQ does: (soz, can't find the link about AggregatedException)
            var crashCount = 0;

            var gridDims = new []{grid.GridDim.Z, grid.GridDim.Y, grid.GridDim.X};
            var numBlocks = gridDims.Product();
            var workers = 0.UpTo(Config.Cores - 1).Select(i => new Thread(() =>
            {
                var chunkSize = (int)Math.Ceiling(numBlocks * 1.0 / Config.Cores);
                var start = i * chunkSize;
                var end = Math.Min((i + 1) * chunkSize, (int)numBlocks) - 1;

                start.UpTo(end).ForEach(j =>
                {
                    var dimSizes = gridDims.Scanrae(1, (curr, dim, _) => curr * dim).ToReadOnly();
                    var indices = dimSizes.SkipLast(1).Scanrbi(j, (curr, dimSize, _) => curr % dimSize, (curr, dimSize, _) => curr / dimSize, (curr, _) => curr).ToReadOnly();
                    var blid = new int3(indices[2], indices[1], indices[0]);

                    try
                    {
                        var blockRunner = kernel.AssertCast<IBlockRunner>();
                        blockRunner.RunBlock(blid);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref crashCount);
                        throw new KernelThreadException(kernel, GridDim, blid, BlockDim, null, Thread.CurrentThread.Name, ex);
                    }
                });
            // todo. also mess with thread affinities to ensure maximally possible CPU load
            }){Name = "Conflux CPU runtime worker thread #" + i}).ToReadOnly();

            workers.ForEach(w => w.Start());
            workers.ForEach(w => w.Join());
            (crashCount == 0).AssertTrue();
        }
    }
}
