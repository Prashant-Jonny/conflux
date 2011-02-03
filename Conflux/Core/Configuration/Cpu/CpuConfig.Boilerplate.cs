using System;
using Conflux.Runtime.Common.Registry;
using Conflux.Runtime.Cpu;
using XenoGears.Assertions;

namespace Conflux.Core.Configuration.Cpu
{
    public partial class CpuConfig
    {
        public CpuConfig() {}
        protected CpuConfig(CpuConfig proto) : base(proto) {}
        public CpuConfig Clone() { return ((ICloneable)this).Clone().AssertCast<CpuConfig>(); }

        private static CpuConfig _default = new CpuConfig();
        public static CpuConfig Default
        {
            get { return new CpuConfig(_default); }
            set { _default = value ?? new CpuConfig(); }
        }

        public static CpuConfig Current
        {
            get
            {
                var runtime = Runtimes.Active as CpuRuntime;
                return runtime == null ? null : runtime.Config;
            }
        }
    }
}
