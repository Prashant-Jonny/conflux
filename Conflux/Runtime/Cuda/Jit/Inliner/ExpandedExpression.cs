using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.Expressions;
using XenoGears.Functional;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    [DebuggerNonUserCode]
    internal class ExpandedExpression
    {
        public ReadOnlyCollection<Node> Stmts { get; private set; }
        public Expression Result { get; private set; }

        public ExpandedExpression(IEnumerable<Node> stmts, Expression result)
        {
            Stmts = stmts.ToReadOnly();
            Result = result;
        }
    }
}