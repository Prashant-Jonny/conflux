using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Conflux.Runtime.Cuda.Jit.Codegen.Layouts;
using Libptx.Common.Enumerations;
using Libptx.Expressions.Slots;
using Libptx.Instructions.Arithmetic;
using Libptx.Instructions.MovementAndConversion;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.TypeInference;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Reflection.Generics;
using HirExpression = Truesight.Decompiler.Hir.Core.Expressions.Expression;
using HirConst = Truesight.Decompiler.Hir.Core.Expressions.Const;
using PtxConst = Libptx.Expressions.Immediate.Const;
using PtxExpression = Libptx.Expressions.Expression;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal partial class Emitter
    {
        public Emitter arrdim(int dim)
        {
            var arr = pop_arr();
            push(arr.Dims[dim]);
            return this;
        }

        public Emitter arrget(MethodBase getter)
        {
            getter.IsArrayGetter().AssertTrue();

            var n_dims = getter.Paramc();
            var indices = 0.UpTo(n_dims - 1).Select(_ => _stk.Pop()).Reverse().ToReadOnly();
            var arr = pop_arr();
            indices.ForEach(idx => _stk.Push(idx));
            calc_offset(arr, n_dims);

            var el_t = arr.Node.Type().GetElementType();
            var d = def_local(el_t);
            var r_offset = (Reg)pop_expr();
            _ptx.Add(new ld{ss = arr.Space, type = el_t, d = d, a = r_offset});
            push(d);

            return this;
        }

        public Emitter arrset(MethodBase setter)
        {
            setter.IsArraySetter().AssertTrue();

            var n_dims = setter.Paramc() - 1;
            var value = pop_expr();
            var indices = 0.UpTo(n_dims - 1).Select(_ => _stk.Pop()).Reverse().ToReadOnly();
            var arr = pop_arr();
            indices.ForEach(idx => _stk.Push(idx));
            calc_offset(arr, n_dims);

            var el_t = arr.Node.Type().GetElementType();
            var r_offset = (Reg)pop_expr();
            _ptx.Add(new st{ss = arr.Space, type = el_t, a = r_offset, b = value});

            return this;
        }

        // assumes that there are n_dims indices pushed onto stack
        // stack balance: in = n_dims, out = 1
        private void calc_offset(ArrayLayout arr, int n_dims)
        {
            var dims = 0.UpTo(n_dims - 1);
            dims.Reverse().ForEach((dim, i) =>
            {
                var @ref = arr.Node.AssertCast<HirExpression>();
                Func<int, HirExpression> get_length = dim1 =>
                {
                    var m_getlength = typeof(Array).GetMethod("GetLength").AssertNotNull();
                    return new Eval(new Apply(new Lambda(m_getlength), @ref, new HirConst(dim1)));
                };

                var e_subprod = (dim + 1).UpTo(n_dims - 1).Fold(null as HirExpression, (curr, j) =>
                {
                    var factor = get_length(j);
                    return curr == null ? factor : Operator.Multiply(curr, factor);
                }) ?? new HirConst(1);
                ld(e_subprod);

                ld("mul.lo");
                if (i != 0) ld("add");
                if (i != n_dims - 1) swap();
            });

            var r_offset = (Reg)pop_expr();
            var el_t = arr.Node.Type().GetElementType();
            var el_sz = Marshal.SizeOf(el_t);
            _ptx.Add(new mul{mode = mulm.lo, type = typeof(int), d = r_offset, a = r_offset, b = new PtxConst(el_sz)});
            push(r_offset);

            var raw = (Reg)arr.Ptr.Offset.Base;
            ld(raw);
            op("add");
        }
    }
}
