using System;
using System.Diagnostics;

namespace Conflux.Runtime.Common.Registry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    internal class RuntimeAttribute : Attribute
    {
        public Platform Platform { get; private set; }

        public RuntimeAttribute(Platform platform)
        {
            Platform = platform;
        }
    }
}