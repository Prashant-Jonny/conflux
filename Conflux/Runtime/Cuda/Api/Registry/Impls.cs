using System;
using System.Diagnostics;
using System.Reflection;
using Conflux.Core.Api.Registry;
using XenoGears.Assertions;
using XenoGears.Collections.Dictionaries;
using XenoGears.Reflection.Attributes;

namespace Conflux.Runtime.Cuda.Api.Registry
{
    [DebuggerNonUserCode]
    internal static class Impls
    {
        private static Object _initLock = new Object();

        static Impls()
        {
            lock (_initLock)
            {
                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                Type = asm.GetTypes().AssertSingle(t => t.HasAttr<ImplAttribute>());
                All = Type.ApiIfacesToImpls();
            }
        }

        public static Type Type { get; private set; }
        public static bool IsImpl(this Type t) { return t == Type; }

        public static ReadOnlyDictionary<MethodBase, MethodBase> All { get; private set; }
        public static bool IsImpl(this MethodBase m) { return All.Values.Contains(m); }
        public static MethodBase ImplFor(this MethodBase m) { return All.AssertSingleOrDefault(kvp => kvp.Value == m).Key; }
    }
}