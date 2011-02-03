using System;
using Conflux.Core.Configuration;
using Conflux.Core.Kernels;

namespace Conflux.Runtime.Common
{
    internal interface IRuntimeJit
    {
        IRuntime Runtime { get; }
        IConfig Config { get; }
        IKernel Compile(Type t_kernel);
    }
}