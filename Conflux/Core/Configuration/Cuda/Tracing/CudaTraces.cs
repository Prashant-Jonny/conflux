using System.Diagnostics;

namespace Conflux.Core.Configuration.Cuda.Tracing
{
    [DebuggerNonUserCode]
    public class CudaTraces
    {
        private FrontEndTraces _frontEnd = new FrontEndTraces();
        public FrontEndTraces FrontEnd { get { return _frontEnd; } }

        private BackEndTraces _backEnd = new BackEndTraces();
        public BackEndTraces BackEnd { get { return _backEnd; } }

        private DriverTraces _ptx = new DriverTraces();
        public DriverTraces Ptx { get { return _ptx; } }
    }
}