using System.Collections.Generic;
using System.Reflection;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Symbols;
using XenoGears.Assertions;

namespace Conflux.Runtime.Cuda.Jit.Malloc
{
    internal class AllocationScheme
    {
        public Dictionary<Sym, MemoryTier> Symbols { get; private set; }
        public Dictionary<FieldInfo, MemoryTier> Fields { get; private set; }

        public AllocationScheme(Dictionary<FieldInfo, MemoryTier> fields, Dictionary<Sym, MemoryTier> symbols)
        {
            Fields = fields;
            Symbols = symbols;
        }

        public MemoryTier this[Ref @ref]
        {
            get
            {
                return this[@ref.AssertNotNull().Sym];
            }
        }

        public MemoryTier this[Sym sym]
        {
            get
            {
                return Symbols[sym];
            }
        }

        public MemoryTier this[Fld fld]
        {
            get
            {
                return this[fld.AssertNotNull().Field];
            }
        }

        public MemoryTier this[FieldInfo fi]
        {
            get
            {
                return Fields[fi];
            }
        }
    }
}