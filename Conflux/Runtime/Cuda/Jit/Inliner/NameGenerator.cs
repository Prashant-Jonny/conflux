using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    [DebuggerNonUserCode]
    internal class NameGenerator
    {
        private readonly HashSet<String> _names = new HashSet<String>();

        public String UniqueName(String prefix)
        {
            var index = 0;
            Func<int, String> nameGen = i => prefix + index;
            while (_names.Contains(nameGen(index))) index++;

            var name = nameGen(index);
            _names.Add(name);
            return name;
        }
    }
}