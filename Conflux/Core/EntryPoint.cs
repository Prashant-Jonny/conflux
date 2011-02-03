using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Conflux.Core.Api.Registry;
using Conflux.Core.Configuration;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Kernels;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Collections.Weak;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Emit2;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Reflection;
using System.Linq;
using XenoGears;
using XenoGears.Streams;

namespace Conflux.Core
{
    public static class EntryPoint
    {
        private static WeakKeyDictionary<Object, IConfig> _kernelConfigs = new WeakKeyDictionary<Object, IConfig>();

        public static TKernel Configure<TKernel>(this AbstractConfig cfg)
            where TKernel : IKernel
        {
            var t_kernel = typeof(TKernel);

            // validate the kernel type to satisfy the criteria
            // more details on this in the spec - here we provide no comments
            // note 1. all runtimes rely on these assumptions!!! => if you change something here, verify all runtimes!!!
            // note 2. annotations and hints are checked separately by every runtime

            typeof(IKernel).IsAssignableFrom(t_kernel).AssertTrue();
            // this is ugly, tho it's the lesser evil (also see comments in Kernel.cs)
            Apis.Ifaces.ForEach(i_api => i_api.IsAssignableFrom(t_kernel).AssertTrue());

            var t_kernels = t_kernel.Unfold(t1 => t1.BaseType, t1 => !t1.SameMetadataToken(typeof(KernelApi))).SkipLast(1).ToReadOnly().AssertNotEmpty();
            t_kernels.ForEach(t1 =>
            {
                (t1.IsClass && t1.IsAbstract).AssertTrue();

                var declared = t1.GetMembers(BF.All | BF.DeclOnly).Where(m => m.Attrs<CompilerGeneratedAttribute>().IsEmpty()).ToReadOnly();
                declared.ForEach(m => m.IsStatic().AssertFalse());
                declared.ForEach(m => (m is FieldInfo || m is PropertyInfo || m is MethodBase).AssertTrue());
                declared.OfType<FieldInfo>().ForEach(fi => fi.IsFamily.AssertTrue());
                declared.OfType<ConstructorInfo>().ForEach(ci => ci.GetParameters().AssertEmpty());
                declared.OfType<MethodInfo>().ForEach(mi =>
                {
                    var aintProtected = !mi.IsFamily;
                    var isVirtual = mi.IsVirtual;
                    var isKernel = mi.Hierarchy().Any(mi2 => mi2.DeclaringType.IsAssignableFrom(typeof(IKernel)));
                    var isKernelApi = mi.Hierarchy().Any(b => Apis.Ifaces.Contains(b.DeclaringType));
                    var isKernelLifecycle = mi.Hierarchy().Any(mi2 => mi2.DeclaringType == typeof(IKernel));

                    isKernelApi.AssertFalse();
                    isKernelLifecycle.AssertImplies(isVirtual);
                    isKernel.AssertEquiv(aintProtected);
                });

                var compilerGenerated = t1.GetMembers(BF.All | BF.DeclOnly).Where(m => m.Attrs<CompilerGeneratedAttribute>().IsNotEmpty());
                var thunks = compilerGenerated.OfType<MethodInfo>().Where(mi =>
                {
                    var match = Regex.Match(mi.Name, @"^\<(?<declaringMethod>.*?)\>.*$");
                    if (!match.Success) return false;

                    var declaringMethod = match.Result("${declaringMethod}");
                    return t1.GetMethods(BF.All | BF.DeclOnly).Any(mi1 => mi1.Name == declaringMethod);
                });
                var cachedAnonymousDelegates = compilerGenerated.OfType<FieldInfo>().Where(fi =>
                    Regex.IsMatch(fi.Name, @"^CS\$\<\>.*?CachedAnonymousMethodDelegate.*$"));
                thunks.AssertEmpty();
                cachedAnonymousDelegates.AssertEmpty();
                compilerGenerated.Except(thunks.Cast<MemberInfo>()).Except(cachedAnonymousDelegates.Cast<MemberInfo>()).AssertEmpty();
            });

            // here we create a totally useless instance of the kernel class which shouldn't be instantiated
            // 
            // this is a consequence of lacking generics+inference features of c# 3.0
            // when C# 4.0 comes, I'll gladly change this sig and the sig below to:
            // * ConfiguredKernel<TKernel> Configure<TKernel>(this BaseConfig cfg) where TKernel : IKernel<T1, T2, R>
            // * R Execute<T1, T2, R>(this IConfiguredKernel<IKernel<T1, T2, R>> kernel, T1 arg1, T2 arg2)
            //
            // the very best of all would be if I could write all this shizzle as follows:
            // * R Execute<TKernel>(this KernelBase<T1, T2, R> kernel, T1 arg1, T2 arg2) where TKernel : IKernel<T1, T2, R>

            var key = typeof(EntryPoint).Assembly.ReadKey("Conflux.Conflux.snk");
            var unit = Codegen.Units["Conflux.Runtime", key];
            var t_concrete = unit.Context.GetOrCreate(t_kernel, () => unit.Module.DefineType(t_kernel.FullName + "_Runtime", TA.Public, t_kernel).CreateType()).AssertCast<Type>();
            var kernel = t_concrete.CreateInstance().AssertCast<TKernel>();

            // todo. here we have a race between a cloning thread and (possibly) modifying thread
            var cloningCtor = cfg.GetType().GetConstructors(BF.PrivateInstance).AssertSingle();
            var cfgClone = cloningCtor.Invoke(cfg.MkArray()).AssertCast<IConfig>();
            _kernelConfigs.Add(kernel, cfgClone);

            return kernel;
        }

        public static R Execute<T1, T2, R>(this Kernel<T1, T2, R> kernel, T1 arg1, T2 arg2)
        {
            var config = _kernelConfigs[kernel];
            var runtime = config.BuildRuntime(kernel.GetType());
            return runtime.Execute(arg1, arg2).AssertCast<R>();
        }

        public static R Execute<T1, R>(this Kernel<T1, R> kernel, T1 arg1)
        {
            var config = _kernelConfigs[kernel];
            var runtime = config.BuildRuntime(kernel.GetType());
            return runtime.Execute(arg1).AssertCast<R>();
        }
    }
}
