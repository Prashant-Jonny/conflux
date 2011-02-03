using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Api;
using Conflux.Runtime.Cuda.Jit.Codegen.Snippets;
using Libcuda.DataTypes;
using Libptx.Bindings;
using Libptx.Instructions.ControlFlow;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.Traversal;
using Truesight.Decompiler.Hir.Traversal.Traversers;
using Truesight.Decompiler.Hir.TypeInference;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection;
using XenoGears.Strings;
using HirConvert = Truesight.Decompiler.Hir.Core.Expressions.Convert;
using HirLabel = Truesight.Decompiler.Hir.Core.ControlFlow.Label;
using PtxLabel = Libptx.Expressions.Addresses.Label;
using Libptx.Common.Types;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal class Generator : AbstractHirTraverser
    {
        private MethodInfo _kernel { get { return JitContext.Current.Kernel; } }
        private CudaConfig _cfg { get { return JitContext.Current.Cfg; } }
        private readonly Emitter _ptx = new Emitter();
        public Emitter Ptx { get { return _ptx; } }

        private PtxLabel _retlabel;
        protected override void TraverseNode(Node node)
        {
            if (Stack.Count() == 1)
            {
                _retlabel = _ptx.def_label("ret");
                base.TraverseNode(node);
                _ptx.label(_retlabel);
                _ptx.emit(new exit{});
                _ptx._stk.AssertEmpty();
            }
            else
            {
                var stmt = !(node is Expression);
                var top_level_expr = node is Expression && !(node.Parent is Expression);
                top_level_expr &= (node.Parent != null); // rule out synthetic expressions (e.g. see TraverseOperator for AndAlso)
                if (stmt || top_level_expr)
                {
                    // note. we comment only top-level expressions
                    // since there's a discrepancy in visit time and emit time
                    // so that typically all comments for intermediate expressions
                    // get emitted much earlier than instructions they refer to

                    _ptx.comment(Environment.NewLine);
                    var i_indent = node.Parents().Count() - 1;
                    var s_indent = i_indent.Times("    ");
                    _ptx.comment(s_indent + node.ToDebugString_WithoutParentInfo());
                }

                base.TraverseNode(node);
            }
        }

        protected override void TraverseAssign(Assign ass)
        {
            var t = ass.Rhs.Type().AssertThat(rhs_t => rhs_t == ass.Lhs.Type());
            (t.IsCudaPrimitive() || t.IsCudaVector()).AssertTrue();

            var lhs_ref = ass.Lhs as Ref;
            if (lhs_ref != null)
            {
                _ptx.ld(ass.Rhs).dup().st(lhs_ref);
            }
            else
            {
                var lhs_eval = ass.Lhs as Eval;
                if (lhs_eval != null)
                {
                    var m = lhs_eval.InvokedMethod();
                    if (m.IsArrayGetter())
                    {
                        var args = lhs_eval.InvocationArgs();
                        args.ForEach(idx => _ptx.ld(idx));
                        _ptx.ld(ass.Rhs);
                        var rhs = _ptx._stk.Pop();
                        _ptx._stk.Push(rhs);
                        _ptx.arrset(m);
                        _ptx._stk.Push(rhs);
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
        }

        protected override void TraverseConditional(Conditional cond)
        {
            var cond_id = Guid.NewGuid().ToString().Slice(0, 4);
            var cond_true = _ptx.def_label("$cond_true_" + cond_id);
            var cond_false = _ptx.def_label("$cond_false_" + cond_id);
            var dnoc = _ptx.def_label("dnoc");

            var t = cond.IfTrue.Type().AssertThat(ift_type => ift_type == cond.IfFalse.Type());
            var result = _ptx.def_local(t);

            _ptx.bra(cond.Test, cond_true)
                .bra(cond_false)
                .label(cond_true)
                .ld(cond.IfTrue)
                .st(result)
                .bra(dnoc)
                .label(cond_false)
                .ld(cond.IfFalse)
                .st(result)
                .label(dnoc)
                .ld(result);
        }

        protected override void TraverseConst(Const @const)
        {
            _ptx.ld((Snippet)@const);
        }

        protected override void TraverseConvert(HirConvert cvt)
        {
            _ptx.ld(cvt.Source).cvt(cvt.Type);
        }

        protected override void TraverseFld(Fld fld)
        {
            var is_static = fld.Field.IsStatic();
            var is_kernel_fld = _kernel.DeclaringType.Hierarchy().Contains(fld.Field.DeclaringType);
            if (!is_static && !is_kernel_fld) _ptx.ld(fld.This);
            _ptx.fld(fld);
        }

        protected override void TraverseOperator(Operator op)
        {
            var lhs = op.Args.FirstOrDefault();
            var rhs = op.Args.SecondOrDefault();
            var targ = op.Args.FirstOrDefault();

            var opt = op.OperatorType;
            if (opt.IsAssign())
            {
                var equiv = op.UnsafeExpandOpAssign();
                _ptx.ld(equiv);
            }
            else if (opt == OperatorType.AndAlso)
            {
                var equiv = new Conditional(lhs, rhs, new Const(false));
                _ptx.ld(equiv);
            }
            else if (opt == OperatorType.OrElse)
            {
                var equiv = new Conditional(lhs, new Const(true), rhs);
                _ptx.ld(equiv);
            }
            else
            {
                op.Args.ForEach(arg => _ptx.ld(arg));

                switch (opt)
                {
                    case OperatorType.Add:
                        _ptx.op("add");
                        break;
                    case OperatorType.And:
                        _ptx.op("and");
                        break;
                    case OperatorType.Divide:
                        _ptx.op("div");
                        break;
                    case OperatorType.Equal:
                        _ptx.op("setp.eq");
                        break;
                    case OperatorType.GreaterThan:
                        _ptx.op("setp.gt");
                        break;
                    case OperatorType.GreaterThanOrEqual:
                        _ptx.op("setp.ge");
                        break;
                    case OperatorType.LeftShift:
                        _ptx.op("shl");
                        break;
                    case OperatorType.LessThan:
                        _ptx.op("setp.lt");
                        break;
                    case OperatorType.LessThanOrEqual:
                        _ptx.op("setp.le");
                        break;
                    case OperatorType.Modulo:
                        _ptx.op("rem");
                        break;
                    case OperatorType.Multiply:
                        var t = _ptx.peek_expr().Type;
                        _ptx.op(t.is_int() ? "mul.lo" : "mul");
                        break;
                    case OperatorType.Negate:
                        _ptx.op("neg");
                        break;
                    case OperatorType.Not:
                        _ptx.ld("not");
                        break;
                    case OperatorType.NotEqual:
                        _ptx.op("setp.ne");
                        break;
                    case OperatorType.Or:
                        _ptx.op("or");
                        break;
                    case OperatorType.RightShift:
                        _ptx.op("shr");
                        break;
                    case OperatorType.Subtract:
                        _ptx.op("sub");
                        break;
                    case OperatorType.Xor:
                        _ptx.op("xor");
                        break;
                    default:
                        throw AssertionHelper.Fail();
                }
            }
        }

        protected override void TraverseRef(Ref @ref)
        {
            _ptx.ld((Snippet)@ref);
        }

        protected override void TraverseEval(Eval eval)
        {
            var m = eval.InvokedMethod();
            var args = eval.InvocationArgsInfo();
            if (m.DeclaringType == typeof(Ctm) && m.Name == "Malloc")
            {
                throw AssertionHelper.Fail();
            }
            else if (m.DeclaringType.IsArray || m.DeclaringType == typeof(Array))
            {
                var @this = args.First().Item1;
                if (m.Name == "GetLength")
                {
                    var e_dim = args.Skip(1).AssertSingle().Item1;
                    var dim = e_dim.AssertCast<Const>().Value.AssertCoerce<int>();
                    _ptx.ld(@this).arrdim(dim);
                }
                else if (m.IsArrayGetter())
                {
                    _ptx.ld(@this);
                    args.Skip(1).ForEach(idx => _ptx.ld(idx.Item1));
                    _ptx.arrget(m);
                }
                else if (m.IsArraySetter())
                {
                    _ptx.ld(@this);
                    args.Skip(1).ForEach(idx => _ptx.ld(idx.Item1));
                    _ptx.arrset(m);
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
            else
            {
                var a_ptx = m.AttrOrNull<PtxAttribute>();
                if (a_ptx == null)
                {
                    var p_m = m.EnclosingProperty();
                    if (p_m != null) a_ptx = p_m.AttrOrNull<PtxAttribute>();
                }

                a_ptx.AssertNotNull();
                (a_ptx.Version <= _cfg.Version).AssertTrue();
                (a_ptx.Target <= _cfg.Target).AssertTrue();

                var s_code = a_ptx.Code.Trim();
                var is_literal = !s_code.Contains(" ");
                is_literal.AssertEquiv(args.IsEmpty());

                if (is_literal)
                {
                    _ptx.op(s_code);
                }
                else
                {
                    var splits = s_code.Split(" ".MkArray(), StringSplitOptions.RemoveEmptyEntries);
                    splits.AssertThat(seq => seq.Count() >= 3);

                    var cop = splits.AssertFirst();
                    splits.AssertSecond().AssertThat(s => s == "%");
                    splits.Skip(2).ForEach(s_arg =>
                    {
                        if (!s_arg.StartsWith("%"))
                        {
                            _ptx.ld(s_arg);
                        }
                        else
                        {
                            var p_name = s_arg.AssertExtract("$%(?<name>.*)^");
                            var p = m.GetParameters().AssertSingle(p1 => p1.Name == p_name);
                            _ptx.ld(args[p]);
                        }
                    });

                    _ptx.op(cop);
                }
            }
        }

        protected override void TraverseBlock(Block block)
        {
            block.ForEach(stmt => _ptx.emit(stmt));
        }

        private Dictionary<Loop, PtxLabel> _breaks = new Dictionary<Loop, PtxLabel>();
        protected override void TraverseBreak(Break @break)
        {
            var loop = @break.Parents().OfType<Loop>().AssertFirst();
            _ptx.bra(_breaks[loop]);
        }

        private Dictionary<Loop, PtxLabel> _continues = new Dictionary<Loop, PtxLabel>();
        protected override void TraverseContinue(Continue @continue)
        {
            var loop = @continue.Parents().OfType<Loop>().AssertFirst();
            _ptx.bra(_continues[loop]);
        }

        protected override void TraverseGoto(Goto @goto)
        {
            _ptx.bra(labels.GetOrCreate(@goto.LabelId, () => _ptx.def_label(@goto.ResolveLabel().Name)));
        }

        protected override void TraverseIf(If @if)
        {
            var if_id = Guid.NewGuid().ToString().Slice(0, 4);
            var if_true = _ptx.def_label("$if_true_" + if_id);
            var if_false = _ptx.def_label("$if_false_" + if_id);
            // you get additional points if you can explain
            // why this variable is named "fi" =)
            var fi = _ptx.def_label("$fi_" + if_id);

            _ptx.bra(@if.Test, if_true)
                .bra(if_false)
                .label(if_true)
                .emit(@if.IfTrue)
                .bra(fi)
                .label(if_false)
                .emit(@if.IfFalse)
                .label(fi);
        }

        private Dictionary<Guid, PtxLabel> labels = new Dictionary<Guid, PtxLabel>();
        protected override void TraverseLabel(HirLabel label)
        {
            _ptx.label(labels.GetOrCreate(label.Id, () => _ptx.def_label(label.Name)));
        }

        protected override void TraverseLoop(Loop loop)
        {
            var loop_id = Guid.NewGuid().ToString().Slice(0, 4);
            var test = _ptx.def_label("$loop_test_" + loop_id);
            var @continue = _ptx.def_label("$loop_continue_" + loop_id);
            var body = _ptx.def_label("$loop_body_" + loop_id);
            var @break = _ptx.def_label("$loop_break_" + loop_id);
            _continues.Add(loop, @continue);
            _breaks.Add(loop, @break);

            _ptx.emit(loop.Init);
            if (loop.IsDoWhile) _ptx.bra(body);

            _ptx.label(test)
                .comment(Environment.NewLine)
                .comment(loop.Test.ToDebugString_WithoutParentInfo())
                .bra(Operator.Not(loop.Test), @break)
                .label(body)
                .emit(loop.Body)
                .label(@continue)
                .emit(loop.Iter)
                .bra(test)
                .label(@break);
        }

        protected override void TraverseReturn(Return @return)
        {
            _ptx.bra(_retlabel);
        }
    }
}