using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using XenoGears.Collections.Dictionaries;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Reflection.Attributes;
using XenoGears.Traits.Disposable;

namespace Conflux.Runtime.Common.Registry
{
    [DebuggerNonUserCode]
    internal static class Runtimes
    {
        private static Object _initLock = new Object();
        public static ReadOnlyDictionary<Platform, Type> All { get; private set; }

        static Runtimes()
        {
            lock (_initLock)
            {
                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                var all = asm.GetTypes().Where(t => t.HasAttr<RuntimeAttribute>()).ToReadOnly();
                All = all.ToDictionary(t => t.Attr<RuntimeAttribute>().Platform, t => t).ToReadOnly();
            }
        }

        // todo. I wonder whether this is a correct way to track runtimes
        // 1) can a thread be switched mid-way during execution of the runtime?
        // 2) won't I hurt the CLR by providing a strong reference to Thread instances?

        private static Object _currentLock = new Object();
        private static Dictionary<Thread, IRuntime> _active = new Dictionary<Thread, IRuntime>();
        public static IRuntime Active { get { lock (_currentLock) { return _active.GetOrDefault(Thread.CurrentThread); } } }
        public static IRuntimeApi ActiveApi { get { return Active.AssertCast<IRuntimeApi>(); } }

        public static IDisposable Activate(IRuntime runtime)
        {
            lock (_currentLock)
            {
                _active.ContainsKey(Thread.CurrentThread).AssertFalse();
                _active.ContainsValue(runtime).AssertFalse();
                _active.Add(Thread.CurrentThread, runtime);
                return new DisposableAction(() => Deactivate(runtime));
            }
        }

        public static void Deactivate(IRuntime runtime)
        {
            lock (_currentLock)
            {
                _active.ContainsKey(Thread.CurrentThread).AssertTrue();
                ReferenceEquals(_active[Thread.CurrentThread], runtime).AssertTrue();
                _active.Remove(Thread.CurrentThread);
            }
        }
    }
}