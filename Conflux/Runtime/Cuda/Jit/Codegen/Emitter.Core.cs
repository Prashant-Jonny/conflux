using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Jit.Codegen.Layouts;
using Libcuda.DataTypes;
using Libptx.Common.Comments;
using Libptx.Common.Types;
using Libptx.Expressions;
using Libptx.Expressions.Slots;
using Libptx.Instructions;
using Libptx.Instructions.ComparisonAndSelection;
using Libptx.Instructions.ControlFlow;
using Libptx.Instructions.Miscellaneous;
using Libptx.Instructions.MovementAndConversion;
using Libptx.Instructions.SynchronizationAndCommunication;
using Libptx.Statements;
using Truesight.Decompiler.Hir.Core.Expressions;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Reflection.Shortcuts;
using HirExpression = Truesight.Decompiler.Hir.Core.Expressions.Expression;
using HirConst = Truesight.Decompiler.Hir.Core.Expressions.Const;
using PtxConst = Libptx.Expressions.Immediate.Const;
using PtxExpression = Libptx.Expressions.Expression;
using PtxModule = Libptx.Module;
using PtxSlot = Libptx.Expressions.Slots.Slot;
using PtxType = Libptx.Common.Types.Type;
using Libptx.Reflection;
using XenoGears.Functional;
using Type=System.Type;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal partial class Emitter
    {
        private static CudaConfig Cfg { get { return JitContext.Current.Cfg; } }
        private static MethodInfo Kernel { get { return JitContext.Current.Kernel; } }
        private PtxModule _module { get { return JitContext.Current.Module; } }
        internal IList<Statement> _ptx { get { return _module.Entries.AssertSingle().Stmts; } }
        private Allocator _alloc { get { return JitContext.Current.Allocator; } }
        private Generator _gen { get { return JitContext.Current.Generator; } }

        public Emitter comment(String comment) { _ptx.Add(new Comment { Text = comment }); return this; }
        public Reg def_local(Type t) { return new Reg { Type = t }; }
        public Reg def_local(Type t, String name) { return new Reg { Type = t, Name = name }; }
        public Emitter def_local(Type t, out Reg reg) { reg = def_local(t); return this; }
        public Emitter def_local(Type t, String name, out Reg reg) { reg = def_local(t, name); return this; }

        public Emitter ld(Ref @ref)
        {
            var layout = _alloc[@ref];

            var lt_slot = layout as SlotLayout;
            if (lt_slot != null)
            {
                var slot = lt_slot.Slot;
                if (slot is Reg)
                {
                    var reg = (Reg)slot;
                    push(reg);
                }
                else if (slot is Var)
                {
                    var @var = (Var)slot;
                    var type = @var.Type;
                    var reg = def_local(type);
                    _ptx.Add(new ld{ss = @var.Space, type = type, d = reg, a = @var});
                    push(reg);
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
            else
            {
                push(layout);
            }

            return this;
        }

        public Emitter ld(Const @const)
        {
            var ptx = new PtxConst(@const.Value);
            push(ptx);
            return this;
        }

        public Emitter op(ptxop ptxop)
        {
            // todo. we need more generic solution
            // btw: set, tex, atom, bar_red and video instructions are left uncovered
            // since they (as well as setp and bar_sync) have variable number of operands

            if (ptxop is setp)
            {
                var setp = (setp)ptxop;
                setp.p = def_local(typeof(bool));
                setp.b = pop_expr();
                setp.a = pop_expr();
                setp.b.Type.agree(setp.a.Type).AssertTrue();
                setp.type = setp.b.Type;
                push(setp.p);
            }
            else if (ptxop is bar_sync)
            {
                var bar_sync = (bar_sync)ptxop;
                bar_sync.a = pop_expr();
            }
            else
            {
                var sample = ptxop.PtxopSigs().AssertFirst();
                var fixup = sample.Destination != null ? 1 : 0;
                var argc = sample.Operands.Count - fixup;
                var args = argc.Times(_ => pop_expr()).Reverse().ToReadOnly();

                // todo. not all instructions are as simple as the lines below assume
                // I mean: cvt, set, slct, suld_b, suld_p, sured_b, sured_p, sust_b, sust_p, tex and video
                var t = args.Fold(null as PtxType, (t_curr, a) => (t_curr ?? a.Type).AssertThat(t1 => t1.agree(a.Type)));
                var p_t = sample.Affixes.SingleOrDefault(p => p.Decl.PropertyType == typeof(PtxType));
                if (p_t != null) p_t.Decl.SetValue(ptxop, t, null);

                if (sample.Destination != null) { var dest = def_local(t); ptxop.Operands[0] = dest; push(dest); }
                args.ForEach((arg, i) => ptxop.Operands[i + fixup] = arg);
            }

            _ptx.Add(ptxop);
            return this;
        }

        public Emitter st(Reg reg)
        {
            var src = pop_expr();
            src.Type.agree(reg.Type).AssertTrue();
            _ptx.Add(new mov{type = reg.Type, d = reg, a = src});
            return this;
        }

        public Emitter st(Var @var)
        {
            var src = pop_expr();
            src.Type.agree(@var.Type).AssertTrue();
            _ptx.Add(new st{ss = @var.Space, type = @var.Type, a = @var, b = src});
            return this;
        }

        public Emitter fld(Fld fld)
        {
            var fi = fld.Field;
            if (fi.DeclaringType.IsCudaVector())
            {
                var target = pop_expr();
                var mod = typeof(Mod).GetFields(BF.PublicStatic).AssertSingle(p => p.Name == fi.Name).GetValue(null).AssertCast<Mod>();
                var modded = target.mod(mod);
                push(modded);
            }
            else if (Kernel.DeclaringType.Hierarchy().Contains(fi.DeclaringType))
            {
                var layout = _alloc[fld];
                push(layout);
            }
            else
            {
                throw AssertionHelper.Fail();
            }

            return this;
        }

        public Emitter cvt(Type dest)
        {
            var a = pop_expr();
            var d = def_local(dest);
            _ptx.Add(new cvt{dtype = dest, atype = a.Type, d = d, a = a});
            push(d);
            return this;
        }
    }
}
