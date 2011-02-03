using System;
using System.Diagnostics;
using System.Reflection.Emit;
using Conflux.Core.Configuration.Cpu;
using Conflux.Runtime.Common;
using Conflux.Runtime.Cpu.Jit;

namespace Conflux.Runtime.Cpu
{
    [DebuggerNonUserCode]
    internal class CpuRuntimeJit : BaseRuntimeJit<CpuRuntime, CpuConfig>
    {
        public CpuRuntimeJit(CpuRuntime runtime)
            : base(runtime)
        {
        }

        protected override void CustomCompile(Type t_kernel, TypeBuilder t)
        {
            JitCompiler.DoCrosscompile(Config, t_kernel, t);
        }
    }
}