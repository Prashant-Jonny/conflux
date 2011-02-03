using System;
using Conflux.Runtime.Common.Registry;
using Conflux.Runtime.Cuda;
using Libcuda;
using XenoGears.Assertions;

namespace Conflux.Core.Configuration.Cuda
{
    public partial class CudaConfig
    {
        public CudaConfig() {}
        protected CudaConfig(CudaConfig proto) : base(proto) {}
        public CudaConfig Clone() { return ((ICloneable)this).Clone().AssertCast<CudaConfig>(); }

        static CudaConfig() { CudaDriver.Ensure(); }
        private static CudaConfig _default = new CudaConfig();
        public static CudaConfig Default
        {
            get { return new CudaConfig(_default); }
            set { _default = value ?? new CudaConfig(); }
        }

        public static CudaConfig Current
        {
            get
            {
                var runtime = Runtimes.Active as CudaRuntime;
                return runtime == null ? null : runtime.Config;
            }
        }
    }
}
