using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Jit.Inliner;
using Conflux.Runtime.Cuda.Jit.Malloc;
using Conflux.Tracing;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using XenoGears.Logging;
using XenoGears.Reflection;
using XenoGears.Assertions;
using XenoGears.Traits.Dumpable;
using System.Linq;
using XenoGears.Functional;
using XenoGears.Strings;
using Conflux.Runtime.Cuda.Jit.Codegen;
using XenoGears.Reflection.Attributes;

namespace Conflux.Runtime.Cuda.Jit
{
    internal static class JitCompiler
    {
        private static MethodInfo Kernel { get { return JitContext.Current.Kernel; } }
        private static Block Hir { get { return JitContext.Current.Hir; } set { JitContext.Current.Hir = value; } }
        private static AllocationScheme Allocs { get { return JitContext.Current.AllocationScheme; } }
        private static Generator Generator { get { return JitContext.Current.Generator; } }
        private static String Ptx { get { return JitContext.Current.Ptx; } }
        private static LevelLogger Log { get { return Traces.Cuda.Info; } }

        // todo. cache jitted kernels
        public static JittedKernel DoCompile(CudaConfig cfg, Type t_kernel)
        {
            t_kernel = t_kernel.Hierarchy().AssertFirst(t => !t.Assembly.HasAttr<CompilerGeneratedAttribute>());
            using (var ctx = new JitContext(cfg, t_kernel))
            {
                var inliner_ctx = new ExpansionContext(Kernel);
                Hir = Hir.Expand(inliner_ctx);
                Log.EnsureBlankLine();
                Log.WriteLine("After inlining:");
                Log.WriteLine(Hir.DumpAsText());

                MemoryAllocator.InferAllocationScheme();
                Log.EnsureBlankLine();
                Log.WriteLine("Non-standard allocations:");
                var nonstandard_allocs = 0;
                Allocs.Fields.Where(kvp => kvp.Value != MemoryTier.Global).ForEach(kvp => { Log.WriteLine(kvp.Key.GetCSharpRef(ToCSharpOptions.Informative)); nonstandard_allocs++; });
                Allocs.Symbols.Where(kvp => kvp.Value != MemoryTier.Private).ForEach(kvp => { Log.WriteLine(kvp.Key); nonstandard_allocs++; });
                Log.WriteLine((nonstandard_allocs == 0 ? "None" : "") + Environment.NewLine);

                // todo. also implement the following:
                // 1) downgrade to SSA
                // 2) perform SCC from "Constant Propagation with Conditional Branches"
                //    when performing SCC don't forget to funcletize stuff e.g. Impl::Cfg and Impl::Device
                // 3) eliminate dead code

                Generator.Traverse(Hir);
                Log.EnsureBlankLine();
                Log.WriteLine("Generated PTX:");
                Log.WriteLine();
                Log.WriteLine(Ptx);

                return new JittedKernel(ctx);
            }
        }
    }
}
