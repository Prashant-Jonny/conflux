using System;
using System.Diagnostics;

namespace Conflux.Core.Annotations.Sharing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    public class LocalAttribute : Attribute
    {
    }
}