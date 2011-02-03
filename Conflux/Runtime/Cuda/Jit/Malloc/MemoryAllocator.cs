using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conflux.Core.Annotations;
using Conflux.Core.Annotations.Sharing;
using Conflux.Core.Configuration.Cuda;
using Conflux.Core.Kernels;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.Core.Scopes;
using Truesight.Decompiler.Hir.Core.Symbols;
using Truesight.Decompiler.Hir.Traversal;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Shortcuts;

namespace Conflux.Runtime.Cuda.Jit.Malloc
{
    internal class MemoryAllocator
    {
        private CudaConfig _cfg { get { return JitContext.Current.Cfg; } }
        private Block _hir { get { return JitContext.Current.Hir; } }
        private AllocationScheme _allocs { get { return JitContext.Current.AllocationScheme; } set { JitContext.Current.AllocationScheme = value; } }

        public static void InferAllocationScheme() { new MemoryAllocator(); }
        private MemoryAllocator()
        {
            // infer allocation hints for fields
            var fai = new Dictionary<FieldInfo, MemoryTier>();
            var t_kernel = CudaRuntime.Current.TKernel;
            var hierarchy = t_kernel.Hierarchy().TakeWhile(t =>
                t.Assembly != typeof(Kernel<,>).Assembly && !t.Name.StartsWith("Kernel`"));
            var fields = hierarchy.SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly)).ToReadOnly();
            fields.ForEach(f => fai.Add(f, f.HasAttr<PrivateAttribute>() ? MemoryTier.Private :
                f.HasAttr<LocalAttribute>() ? MemoryTier.Shared :
                f.HasAttr<GlobalAttribute>() ? MemoryTier.Global :
                /* otherwise, default to mem_global */ MemoryTier.Global));
            // todo. we don't support non-global fields to be consistent with CPU runtime
            fai.AssertNone(kvp => kvp.Value != MemoryTier.Global);

            // infer allocation hints for locals
            var sai = new Dictionary<Sym, MemoryTier>();
            var locals = _hir.LocalsRecursive();
            locals.ForEach(local => sai.Add(local, 0));

            var evals = _hir.Family().OfType<Eval>().ToReadOnly();
            foreach (var eval in evals)
            {
                var m = eval.InvokedMethod();
                var t = m == null ? null : m.DeclaringType;
                if (t != null && t.DeclaringType == typeof(Hints))
                {
                    // todo. support other hints as well
                    (t == typeof(Hints.SharingHint)).AssertTrue();
                    var mem = m == t.GetMethod("Private") ? MemoryTier.Private :
                        m == t.GetMethod("Local") ? MemoryTier.Shared :
                        // todo. we don't support global sharing for locals to be consistent with CPU runtime
                        m == t.GetMethod("Global") ? ((Func<MemoryTier>)(() => { throw AssertionHelper.Fail(); }))() :
                        ((Func<MemoryTier>)(() => { throw AssertionHelper.Fail(); }))();

                    var args = eval.Callee.Args.AsEnumerable();
                    args = args.Skip(1.AssertThat(_ => m.IsInstance()));
                    m.IsVarargs().AssertTrue();
                    args = args.AssertSingle().AssertCast<CollectionInit>().Elements;

                    foreach (var arg in args)
                    {
                        // todo. we don't support hints for fields to be consistent with CPU runtime
                        var @ref = arg.AssertCast<Ref>();
                        var sym = @ref.Sym.AssertThat(s => s.IsLocal());
                        (sai[sym] == 0).AssertTrue();
                        sai[sym] = mem;
                    }
                }
            }

            // if no memory model has been explicitly specified for the local, we default to mem_private
            locals.Where(local => sai[local] == 0).ForEach(local => sai[local] = MemoryTier.Private);
            _allocs = new AllocationScheme(fai, sai);
        }
    }
}
