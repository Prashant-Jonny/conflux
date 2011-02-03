using System;
using System.Diagnostics;
using Conflux.Core.Configuration;
using Conflux.Runtime.Common.Registry;
using XenoGears.Reflection.Attributes;

namespace Conflux.Runtime.Common
{
    [DebuggerNonUserCode]
    internal abstract class AbstractRuntime : IRuntime
    {
        public Platform Platform { get { return this.GetType().Attr<RuntimeAttribute>().Platform; } }
        public IConfig Config { get; private set; }
        public Type TKernel { get; private set; }

        protected AbstractRuntime(IConfig config, Type t_kernel)
        {
            Config = config;
            TKernel = t_kernel;
        }

        public abstract Object Execute(params Object[] args);
        Object IRuntime.Execute(params Object[] args)
        {
            using (Runtimes.Activate(this))
            {
                return Execute(args);
            }
        }
    }
}