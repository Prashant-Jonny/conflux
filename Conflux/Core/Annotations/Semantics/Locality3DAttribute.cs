using System;
using System.Diagnostics;

namespace Conflux.Core.Annotations.Semantics
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    public class Locality3DAttribute : Attribute
    {
    }
}