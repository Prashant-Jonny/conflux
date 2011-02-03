using System;
using Libptx.Common;
using Libptx.Expressions.Immediate;
using Libptx.Expressions.Sregs;
using Libptx.Instructions;
using Libptx.Reflection;
using Truesight.Decompiler.Hir;
using XenoGears.Assertions;
using ptxexpr = Libptx.Expressions.Expression;
using XenoGears.Reflection;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Snippets
{
    internal abstract class Snippet
    {
        public abstract int In { get; }
        public abstract int Out { get; }
        public abstract Type Type { get; }

        public static implicit operator Snippet(String inline_ptx)
        {
            if (inline_ptx == null) return null;
            if (inline_ptx == "WARP_SZ") return new WarpSz();
            else if (inline_ptx.StartsWith("%")) return Sregs.All.AssertSingle(sreg => sreg.SregSig().Name == inline_ptx).CreateInstance().AssertCast<Sreg>();
            else return new PtxopSnippet(inline_ptx);
        }

        public static implicit operator Snippet(Atom atom)
        {
            var ptxop = atom as ptxop;
            if (ptxop != null)
            {
                return new PtxopSnippet(ptxop);
            }

            var ptxexpr = atom as ptxexpr;
            if (ptxexpr != null)
            {
                return new PtxexprSnippet(ptxexpr);
            }

            throw AssertionHelper.Fail();
        }

        public static implicit operator Snippet(Node node)
        {
            return new NodeSnippet(node);
        }
    }
}