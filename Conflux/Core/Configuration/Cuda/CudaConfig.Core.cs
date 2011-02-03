using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conflux.Core.Annotations;
using Conflux.Core.Api.Registry;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Configuration.Common.Registry;
using Conflux.Core.Configuration.Cuda.Codebase;
using Conflux.Core.Configuration.Cuda.Tracing;
using Conflux.Runtime;
using Conflux.Runtime.Cuda.Api;
using Conflux.Runtime.Cuda.Api.Registry;
using Libcuda;
using Libcuda.Versions;
using Truesight.Decompiler.Hir.Core.Expressions;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Reflection;

namespace Conflux.Core.Configuration.Cuda
{
    [Config(Platform.Cuda)]
    [DebuggerNonUserCode]
    public partial class CudaConfig : BaseConfig
    {
        private bool _downgradeDoubles = false;
        public bool DowngradeDoubles
        {
            get { return _downgradeDoubles; }
            set
            {
                (value == true).AssertImplies(Target < HardwareIsa.SM_13);
                _downgradeDoubles = value;
            }
        }

        private SoftwareIsa _version = CudaVersions.SoftwareIsa;
        public SoftwareIsa Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private HardwareIsa _target = CudaVersions.HardwareIsa;
        public HardwareIsa Target
        {
            get { return _target; }
            set
            {
                (value >= HardwareIsa.SM_13).AssertImplies(DowngradeDoubles != true);
                _target = value;
            }
        }

        private CudaCodebase _codebase = DefaultCodebase();
        public CudaCodebase Codebase { get { return _codebase; } set { _codebase = value; } }
        private static CudaCodebase DefaultCodebase()
        {
            var cb = new CudaCodebase();
            cb.Ignore(typeof(Object).GetConstructor(Type.EmptyTypes));
            cb.Redirect((MethodBase m) => m.IsApi(), m => Impls.All[m.Api()]);
            cb.Redirect((MethodBase m) => m.IsConstructor() && m.DeclaringType.IsArray,
                (m, args) => typeof(Ctm).GetMethod("Malloc", typeof(Type).Concat(args.Count().Times(typeof(int))).ToArray()).AssertNotNull(),
                (m, args) => new Const(m.DeclaringType.GetElementType()).Concat(args));
            cb.Special(t => t == typeof(Array) || t.IsArray);
            cb.Special(typeof(AssertionHelper));
            cb.Special((Type t) => t == typeof(Hints) || t.DeclaringType == typeof(Hints));
            cb.Special(typeof(CudaDevices));
            cb.Special(typeof(CudaConfig));
            cb.Special(typeof(Ctm));
            cb.OptIn(t => t.Assembly.GetName().Name == "XenoGears");
            cb.OptIn(t => t.Assembly.GetName().Name == "Conflux");
            cb.OptOut(typeof(Math));
            return cb;
        }

        private CudaTraces _traces = DefaultTraces();
        public CudaTraces Traces { get { return _traces; } set { _traces = value; } }
        private static CudaTraces DefaultTraces()
        {
            // todo. implement this:
            // 1) populate *Traces classes
            // 2) implement fluent interface for those ala done for Hints
            // 3) implement defaults for traces (i.e. this very method)
            // 4) implement the same for CPU runtime
            // 5) kill LameCpu runtime from public API (i.e. make LameCpuConfig to be internal)
            return new CudaTraces();
        }
    }
}