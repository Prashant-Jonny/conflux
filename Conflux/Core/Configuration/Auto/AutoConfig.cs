using System.Diagnostics;
using Conflux.Core.Configuration.Common;
using Conflux.Core.Configuration.Common.Registry;
using Conflux.Runtime;

namespace Conflux.Core.Configuration.Auto
{
    [Config(Platform.Auto)]
    [DebuggerNonUserCode]
    public class AutoConfig : BaseConfig
    {
        public AutoConfig()
        {
        }

        protected AutoConfig(AutoConfig proto)
            : base(proto)
        {
        }
    }
}