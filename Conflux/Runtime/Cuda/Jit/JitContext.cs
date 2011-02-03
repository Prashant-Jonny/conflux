using System;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Jit.Codegen;
using Conflux.Runtime.Cuda.Jit.Malloc;
using Libptx.Functions;
using Truesight.Decompiler;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using XenoGears;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Reflection.Generics;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Traits.Disposable;
using PtxModule = Libptx.Module;

namespace Conflux.Runtime.Cuda.Jit
{
    internal class JitContext : Disposable
    {
        public CudaConfig Cfg { get; private set; }
        public MethodInfo Kernel { get; private set; }

        public Block Hir { get; set; }
        public AllocationScheme AllocationScheme { get; set; }
        public Allocator Allocator { get; set; }
        public Generator Generator { get; set; }
        public PtxModule Module { get; set; }
        public Entry Entry { get; set; }
        public Params Params { get; set; }
        public String Ptx { get { return Module.RenderPtx(); } }

        [ThreadStatic] public static JitContext Current;
        public JitContext(CudaConfig cfg, Type t_kernel)
        {
            Current.AssertNull();
            Current = this;

            Cfg = cfg;
            Kernel = t_kernel.GetMethod("RunKernel", BF.All);
            Kernel.AssertNotNull();
            Kernel.IsInstance().AssertTrue();
            Kernel.Params().AssertEmpty();
            (Kernel.Ret() == typeof(void)).AssertTrue();

            Hir = Kernel.Decompile().Body;
            AllocationScheme = null;
            Allocator = new Allocator();
            Generator = new Generator();
            Module = new PtxModule(Cfg.Target, Cfg.Version);
            Entry = Module.AddEntry(Kernel.Name);
            Params = Entry.Params;
        }

        protected override void DisposeManagedResources()
        {
            (Current == this).AssertTrue();
            Current = null;
        }
    }
}