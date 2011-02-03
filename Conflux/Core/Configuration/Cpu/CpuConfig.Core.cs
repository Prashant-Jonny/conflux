using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Configuration.Common.Registry;
using Conflux.Runtime;
using XenoGears.Assertions;

namespace Conflux.Core.Configuration.Cpu
{
    [Config(Platform.Cpu)]
    [DebuggerNonUserCode]
    public partial class CpuConfig : BaseConfig
    {
        private int _cores = Environment.ProcessorCount;
        public int Cores
        {
            get { return _cores; }
            set
            {
                (value > 0).AssertTrue();
                _cores = value;
            }
        }

        private bool _emitDebuggableIL = false;
        public bool EmitDebuggableIL
        {
            get { return _emitDebuggableIL; }
            set { _emitDebuggableIL = value; }
        }
    }
}