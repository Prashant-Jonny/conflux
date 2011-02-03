using System;
using ptxexpr = Libptx.Expressions.Expression;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Snippets
{
    internal class PtxexprSnippet : Snippet
    {
        public ptxexpr Expr { get; private set; }
        public PtxexprSnippet(ptxexpr expr) { Expr = expr; }

        public override int In { get { return 0; } }
        public override int Out { get { return 1; } }
        public override Type Type { get { return Expr.Type; } }
    }
}