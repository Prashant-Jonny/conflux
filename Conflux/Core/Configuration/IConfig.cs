using System;
using Conflux.Runtime;

namespace Conflux.Core.Configuration
{
    internal interface IConfig : ICloneable
    {
        Platform Platform { get; }
        IRuntime BuildRuntime(Type t_kernel);
    }
}