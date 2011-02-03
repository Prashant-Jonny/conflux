using System;
using System.Diagnostics;

namespace Conflux.Core.Annotations.Alloc
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    public class SpilledAttribute : Attribute
    {
    }
}