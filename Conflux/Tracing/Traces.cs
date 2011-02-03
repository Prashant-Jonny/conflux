using System.Diagnostics;
using XenoGears.Logging;

namespace Conflux.Tracing
{
    [DebuggerNonUserCode]
    public static class Traces
    {
        public readonly static Logger Runtime = Logger.Get("Conflux.Runtime");
        public readonly static Logger Cpu = Logger.Get("Conflux.Runtime.Cpu");
        public readonly static Logger Cuda = Logger.Get("Conflux.Runtime.Cuda");
    }
}