using System;
using System.Collections.Generic;
using Conflux.Runtime.Cuda.Jit.Codegen.Layouts;
using Conflux.Runtime.Cuda.Jit.Codegen.Snippets;
using Libptx.Common;
using Libptx.Expressions.Slots;
using Libptx.Expressions.Sregs;
using Libptx.Instructions.MovementAndConversion;
using Truesight.Decompiler.Hir.Core.Expressions;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Strings;
using HirExpression = Truesight.Decompiler.Hir.Core.Expressions.Expression;
using PtxExpression = Libptx.Expressions.Expression;
using Libptx.Reflection;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal partial class Emitter
    {
        internal Stack<Object> _stk = new Stack<Object>();
        private void push(Layout slot) { _stk.Push(slot); }
        private void push(PtxExpression expr) { _stk.Push(expr); }
        internal ArrayLayout peek_arr() { return _stk.Peek().AssertCast<ArrayLayout>(); }
        internal PtxExpression peek_expr() { var raw = _stk.Peek(); (raw is PtxExpression || raw is ArrayLayout).AssertTrue(); return raw.AssertCast<PtxExpression>(); }
        internal ArrayLayout pop_arr() { return _stk.Pop().AssertCast<ArrayLayout>(); }
        internal PtxExpression pop_expr() { var raw = _stk.Pop(); (raw is PtxExpression || raw is ArrayLayout).AssertTrue(); return raw.AssertCast<PtxExpression>(); }

        public Emitter emit(Snippet stmt) { ld(stmt); stmt.Out.TimesDo(() => _stk.Pop()); return this; }
        public Emitter pop() { _stk.Pop(); return this; }
        public Emitter dup() { _stk.Push(_stk.Peek()); return this; }
        public Emitter swap() { var fst = _stk.Pop(); var snd = _stk.Pop(); _stk.Push(fst); _stk.Push(snd); return this; }

        private Dictionary<SregSig, Reg> _sreg_caches = new Dictionary<SregSig, Reg>();
        public Emitter ld(Snippet src)
        {
            if (src is NodeSnippet)
            {
                var ns = src as NodeSnippet;
                if (ns.Node is Ref) ld((Ref)ns.Node);
                else if (ns.Node is Const) ld((Const)ns.Node);
                else _gen.Traverse(ns.Node);
            }
            else if (src is PtxopSnippet)
            {
                var pos = src as PtxopSnippet;
                op(pos.Ptxop);
            }
            else if (src is PtxexprSnippet)
            {
                var pes = src as PtxexprSnippet;
                if (pes.Expr is Sreg)
                {
                    push(_sreg_caches.GetOrCreate(pes.Expr.SregSig(), () =>
                    {
                        var cache = def_local(pes.Expr.Type, pes.Expr.SregSig().Name.Slice(1));
                        _ptx.Add(new mov{type = pes.Expr.Type, d = cache, a = pes.Expr});
                        return cache;
                    }));
                }
                else
                {
                    push(pes.Expr);
                }
            }
            else
            {
                throw AssertionHelper.Fail();
            }

            return this;
        }

        public Emitter op(String opcode)
        {
            return ld(opcode);
        }

        public Emitter sreg(String sreg)
        {
            return ld(sreg);
        }

        public Emitter st(Snippet dest)
        {
            if (dest is NodeSnippet)
            {
                var ns = dest as NodeSnippet;
                if (ns.Node is Ref)
                {
                    var @ref = (Ref)ns.Node;
                    var layout = _alloc[@ref].AssertCast<SlotLayout>();
                    return st((Atom)layout.Slot);
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
            else if (dest is PtxexprSnippet)
            {
                var pes = dest as PtxexprSnippet;
                if (pes.Expr is Reg) st((Reg)pes.Expr);
                else if (pes.Expr is Var) st((Var)pes.Expr);
                else throw AssertionHelper.Fail();
            }
            else
            {
                throw AssertionHelper.Fail();
            }

            return this;
        }
    }
}
