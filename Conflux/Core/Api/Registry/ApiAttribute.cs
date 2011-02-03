using System;
using System.Diagnostics;

namespace Conflux.Core.Api.Registry
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    internal class ApiAttribute : Attribute
    {
    }
}