using System;
using System.Linq;
using System.Diagnostics;
using Conflux.Core.Annotations.Semantics;
using Conflux.Core.Configuration;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Shortcuts;

namespace Conflux.Core.Kernels
{
    // note. as I've written in the OneNote notebook, 
    // Kernel class implementing IXXXApis interfaces is ugly
    // I'd better not implement those interfaces, but rather provide the functionality
    // via special instances or in static classes (ala Math)
    //
    // however, all these options require a qualifier to be used 
    // (api.SyncThreads() in the first case, and SyncApi.SyncThreads() in the second case)
    // to me that's pretty annoying so I've decided to implement 
    // conceptually impure but convenient thingie
    //
    // if we had static import like in Java, we could get the best of two worlds
    // but so far let's stick to what we have

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, T3, T4, T5, T6, T7, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 7).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, T3, T4, T5, T6, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 6).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, T3, T4, T5, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 5).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, T3, T4, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2, T3 arg3, T4 arg4) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 4).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, T3, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2, T3 arg3) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 3).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1], (T3)args[2]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, T2, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1, T2 arg2) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 2).AssertTrue();
            Initialize(gridCfg, (T1)args[0], (T2)args[1]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<T1, R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg, T1 arg1) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 1).AssertTrue();
            Initialize(gridCfg, (T1)args[0]);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }

    [DebuggerNonUserCode]
    public abstract class Kernel<R> : KernelApi, IKernel
    {
        protected virtual void Initialize(IGridConfig gridCfg) { }
        void IKernel.Initialize(IGridConfig gridCfg, params Object[] args)
        {
            (args != null && args.Count() == 0).AssertTrue();
            Initialize(gridCfg);
        }

        protected abstract void RunKernel();
        void IKernel.RunKernel() { RunKernel(); }

        Object IKernel.FetchResult() { return FetchResult(); }
        protected virtual R FetchResult()
        {
            var resultFields = this.GetType().Hierarchy()
                .SelectMany(t => t.GetFields(BF.AllInstance | BF.DeclOnly))
                .Where(f => f.HasAttr<ResultAttribute>());
            var singleResult = resultFields.SingleOrDefault2().AssertNotNull();
            return singleResult.GetValue(this).AssertCast<R>();
        }
    }
}
