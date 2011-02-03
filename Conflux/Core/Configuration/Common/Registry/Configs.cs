using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conflux.Runtime;
using XenoGears.Collections.Dictionaries;
using XenoGears.Functional;
using XenoGears.Reflection.Attributes;

namespace Conflux.Core.Configuration.Common.Registry
{
    [DebuggerNonUserCode]
    internal static class Configs
    {
        private static Object _initLock = new Object();
        public static ReadOnlyDictionary<Platform, Type> All { get; private set; }

        static Configs()
        {
            lock (_initLock)
            {
                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                var all = asm.GetTypes().Where(t => t.HasAttr<ConfigAttribute>()).ToReadOnly();
                All = all.ToDictionary(t => t.Attr<ConfigAttribute>().Platform, t => t).ToReadOnly();
            }
        }
    }
}