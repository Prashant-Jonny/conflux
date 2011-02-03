using System;
using Conflux.Core.Configuration;

namespace Conflux.Runtime
{
    internal interface IRuntime
    {
        Platform Platform { get; }
        IConfig Config { get; }
        Type TKernel { get; }

        Object Execute(params Object[] args);
    }
}
