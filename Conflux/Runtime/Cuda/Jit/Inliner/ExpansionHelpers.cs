using System.Diagnostics;
using System.Reflection;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.Traversal;
using XenoGears.Assertions;
using XenoGears.Reflection;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    [DebuggerNonUserCode]
    internal static class ExpansionHelpers
    {
        public static bool PassedByRef(this ParameterInfo pi)
        {
            pi.ParameterType.IsPointer.AssertFalse();
            return pi.ParameterType.IsByRef;
        }

        public static bool PassedByValue(this ParameterInfo pi)
        {
            pi.ParameterType.IsPointer.AssertFalse();
            return !pi.PassedByRef();
        }

        public static bool IsLvalue(this Node n)
        {
            var @ref = n as Ref;
            if (@ref != null) return true;

            var fld = n as Fld;
            if (fld != null) return fld.Field.IsInstance();

            var eval = n as Eval;
            if (eval != null)
            {
                var m = eval.InvokedMethod();
                return m.IsArrayGetter();
            }

            return false;
        }
    }
}