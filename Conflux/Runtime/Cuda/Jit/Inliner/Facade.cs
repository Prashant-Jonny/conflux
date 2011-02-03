using System.Diagnostics;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    [DebuggerNonUserCode]
    internal static class Facade
    {
        public static Block Expand(this Block blk, ExpansionContext ctx)
        {
            return BlockExpander.ExpandBlock(blk, ctx);
        }

        public static ExpandedExpression Expand(this Expression expr, ExpansionContext ctx)
        {
            return ExpressionExpander.ExpandExpression(expr, ctx);
        }
    }
}