using System;
using System.Diagnostics;
using Conflux.Runtime;

namespace Conflux.Core.Configuration.Common.Registry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [DebuggerNonUserCode]
    internal class ConfigAttribute : Attribute
    {
        public Platform Platform { get; private set; }

        public ConfigAttribute(Platform platform)
        {
            Platform = platform;
        }
    }
}