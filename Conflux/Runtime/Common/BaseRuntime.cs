using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Kernels;
using XenoGears.Assertions;
using XenoGears.Reflection;

namespace Conflux.Runtime.Common
{
    [DebuggerNonUserCode]
    internal abstract partial class BaseRuntime<TConfig, TJit> : AbstractRuntime, IRuntimeCore, IRuntimeApi
        where TConfig : BaseConfig
        where TJit : IRuntimeJit
    {
        protected TJit Jit { get; private set; }
        public new TConfig Config { get { return base.Config.AssertCast<TConfig>(); } }

        protected BaseRuntime(TConfig config, Type t_kernel)
            : base(config, t_kernel)
        {
            Jit = (TJit)typeof(TJit).CreateInstance(this);
        }

        public override Object Execute(params Object[] args)
        {
            var kernel = Jit.Compile(TKernel);
            kernel.Initialize(Config, args);
            kernel.RunKernel();
            return kernel.FetchResult();
        }

        void IRuntimeCore.CoreRunKernel(IGrid grid, IKernel kernel) { CoreRunKernel(grid, kernel); }
        protected abstract void CoreRunKernel(IGrid grid, IKernel kernel);
    }
}