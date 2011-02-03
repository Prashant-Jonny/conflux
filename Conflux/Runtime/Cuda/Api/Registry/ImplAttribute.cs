using System;
using System.Diagnostics;

namespace Conflux.Runtime.Cuda.Api.Registry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    internal class ImplAttribute : Attribute
    {
    }
}