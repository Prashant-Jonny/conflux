using System;
using System.Diagnostics;
using System.Reflection.Emit;
using Conflux.Core.Api.Registry;
using Conflux.Core.Configuration;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Kernels;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Reflection.Generics;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Reflection.Emit;
using XenoGears.Streams;

namespace Conflux.Runtime.Common
{
    [DebuggerNonUserCode]
    internal abstract class BaseRuntimeJit<TRuntime, TConfig> : IRuntimeJit
        where TRuntime : IRuntime
        where TConfig : BaseConfig
    {
        IRuntime IRuntimeJit.Runtime { get { return Runtime; } }
        public TRuntime Runtime { get; private set; }
        IConfig IRuntimeJit.Config { get { return Config; } }
        public TConfig Config { get { return Runtime.Config.AssertCast<TConfig>(); } }
        protected BaseRuntimeJit(TRuntime runtime) { Runtime = runtime; }

        public IKernel Compile(Type t_kernel)
        {
            var unit_name = String.Format("Conflux.Runtime.{0}.Jit", Runtime.Platform);
            var unit_key = typeof(BaseRuntimeJit<,>).Assembly.ReadKey("Conflux.Conflux.snk");
            var unit = XenoGears.Reflection.Emit2.Codegen.Units[unit_name, unit_key];
            return unit.Context.GetOrCreate(t_kernel, () =>
            {
                // Constructor
                var name = t_kernel.FullName;
                if (name.EndsWith("_Runtime")) name = name.Substring(0, name.Length - "_Runtime".Length);
                var postfix = GetType().Name;
                var t = unit.Module.DefineType(name + "_" + postfix, TA.Public, t_kernel);
                var f_runtime = t.DefineField("_runtime", typeof(TRuntime), FA.Private);
                var ctor = t.DefineConstructor(MA.PublicCtor, CC.Std, typeof(TRuntime).MkArray());
                ctor.DefineParameter(1, ParmA.None, "runtime");
                ctor.il().ldarg(0).ldarg(1).stfld(f_runtime).ret();

                // Redirect implementations of kernel APIs to the runtime
                t_kernel.ApiIfacesToImpls().ForEach(kvp =>
                {
                    var apiDecl = kvp.Key;
                    var baseImpl = kvp.Value;

                    var impl = t.DefineOverride(baseImpl);
                    baseImpl.GetParameters().ForEach((pi, i) => impl.DefineParameter(i + 1, ParmA.None, pi.Name));
                    impl.il()
                        .ldarg(0)
                        .ldfld(f_runtime)
                        .ld_args(1, apiDecl.Paramc())
                        .callvirt(apiDecl)
                        .ret();
                });

                // Redirect implementation of RunKernel to the runtime
                var orig_runkernel = t_kernel.GetMethod("RunKernel", BF.All);
                var m_runkernel = t.OverrideMethod(orig_runkernel);
                var m_corerunkernel = typeof(IRuntimeCore).GetMethod("CoreRunKernel");
                m_runkernel.il()
                    .ldarg(0)
                    .ldfld(f_runtime)
                    .ldarg(0)
                    .ldfld(f_runtime)
                    .callvirt(typeof(TRuntime).GetProperties(BF.All).AssertSingle(p => p.Name == "Config" && p.DeclaringType.Name == typeof(BaseRuntime<,>).Name).GetGetMethod(true))
                    .callvirt(typeof(GridHelpers).GetMethod("ToGrid", new []{typeof(IGridConfig)}))
                    .ldarg(0)
                    .call(m_corerunkernel)
                    .ret();

                // Invoke custom compilation logic if necessary
                CustomCompile(t_kernel, t);

                // false is necessary to ensure that ref.emit won't destroy our symbols
                return t.CreateType(false);
            }).AssertCast<Type>().CreateInstance(Runtime).AssertCast<IKernel>();
        }

        protected virtual void CustomCompile(Type t_kernel, TypeBuilder t)
        {
            // default implementation does nothing
        }
    }
}