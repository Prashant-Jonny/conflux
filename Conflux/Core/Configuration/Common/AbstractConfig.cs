using System;
using System.Diagnostics;
using Conflux.Core.Configuration.Common.Registry;
using Conflux.Runtime;
using Conflux.Runtime.Common.Registry;
using XenoGears.Assertions;
using System.Linq;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Shortcuts;

namespace Conflux.Core.Configuration.Common
{
    [DebuggerNonUserCode]
    public abstract class AbstractConfig : IConfig
    {
        protected AbstractConfig()
        {
        }

        // todo. this creates a race between cloning and modifying threads
        // think carefully and do something about that
        protected AbstractConfig(AbstractConfig proto)
        {
            if (proto != null)
            {
                (proto.GetType() == this.GetType()).AssertTrue();

                var props = this.GetType().GetProperties(BF.AllInstance);
                props.ForEach(p => p.SetValue(this, p.GetValue(proto, null), null));
            }
        }

        Object ICloneable.Clone()
        {
            var copyCtor = GetType().GetConstructors(BF.All)
                .Where(ctor => ctor.GetParameters().Count() == 1)
                .AssertSingle(ctor => ctor.GetParameters().AssertSingle().ParameterType == this.GetType());
            return copyCtor.Invoke(this.MkArray());
        }

        Platform IConfig.Platform
        {
            get { return this.GetType().Attr<ConfigAttribute>().Platform; }
        }

        IRuntime IConfig.BuildRuntime(Type t_kernel)
        {
            var t_runtime = Runtimes.All[((IConfig)this).Platform];
            return t_runtime.CreateInstance(this, t_kernel).AssertCast<IRuntime>();
        }
    }
}