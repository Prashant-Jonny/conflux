using System;
using System.Diagnostics;
using System.Linq;
using Conflux.Core;
using Conflux.Core.Kernels;
using Conflux.Runtime.Cpu;
using Conflux.Runtime.Cuda;
using NUnit.Framework;
using XenoGears.Functional;
using XenoGears.Playground.Framework;
using XenoGears.Reflection;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Generics;
using XenoGears.Strings;

namespace Conflux.Playground
{
    [TestFixture]
    public class MetaTests : BaseTests
    {
        [Test]
        public void EnsureMostStuffIsMarkedWithDebuggerNonUserCode()
        {
            var asm = typeof(IKernel).Assembly;
            var types = asm.GetTypes().Where(t => !t.IsInterface).ToReadOnly();
            var failed_types = types
                .Where(t => !t.HasAttr<DebuggerNonUserCodeAttribute>())
                .Where(t => !t.IsCompilerGenerated())
                .Where(t => !t.Name.Contains("<>"))
                .Where(t => !t.Name.Contains("__StaticArrayInit"))
                .Where(t => !t.IsEnum)
                .Where(t => !t.IsDelegate())
                // exceptions for meaty logic
                .Where(t => t != typeof(EntryPoint))
                .Where(t => t != typeof(CpuRuntime))
                .Where(t => t.Namespace.StartsWith(typeof(global::Conflux.Runtime.Cpu.Jit.JitCompiler).Namespace))
                .Where(t => t != typeof(CudaRuntime))
                .Where(t => t.Namespace.StartsWith(typeof(global::Conflux.Runtime.Cuda.Jit.JitCompiler).Namespace))
                .ToReadOnly();

            if (failed_types.IsNotEmpty())
            {
                Log.WriteLine(String.Format("{0} types in Conflux aren't marked with [DebuggerNonUserCode]:", failed_types.Count()));
                var messages = failed_types.Select(t => t.GetCSharpRef(ToCSharpOptions.InformativeWithNamespaces));
                messages.OrderDescending().ForEach(message => Log.WriteLine(message));
                Assert.Fail();
            }
        }
    }
}