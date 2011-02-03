using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Scopes;
using Truesight.Decompiler.Hir.Core.Symbols;
using XenoGears.Functional;
using XenoGears.Assertions;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    [DebuggerNonUserCode]
    internal class ExpansionContext
    {
        public ExpansionContext Parent { get; private set; }
        public CudaConfig Config { get { return JitContext.Current.Cfg; } }
        public Stack<MethodBase> Stack { get; set; }
        public Scope Scope { get; set; }
        public NameGenerator Names { get; set; }
        public Dictionary<Sym, Expression> Env { get; set; }
        public Ref Ret { get; set; }
        public Label RetLabel { get; set; }

        public ExpansionContext(MethodBase root)
            : this(new Stack<MethodBase>(root.MkArray()))
        {
        }

        public ExpansionContext(Stack<MethodBase> stack)
            : this(stack, null, new NameGenerator(), null)
        {
        }

        private ExpansionContext(Stack<MethodBase> stack, Scope scope, NameGenerator names)
            : this(stack, scope, names, null)
        {
        }

        private ExpansionContext(Stack<MethodBase> stack, Scope scope, NameGenerator names, IEnumerable<KeyValuePair<Sym, Expression>> env)
        {
            Stack = stack;
            Scope = scope;
            Names = names;
            Env = env.ToDictionary();
        }

        public ExpansionContext SpinOff()
        {
            return new ExpansionContext(Stack, Scope, Names, Env){Parent = this, Ret = Ret, RetLabel = RetLabel};
        }

        public ExpansionContext SpinOff(MethodBase callee)
        {
            Stack.Contains(callee).AssertFalse();
            var new_stack = new Stack<MethodBase>();
            callee.Concat(Stack).Reverse().ForEach(new_stack.Push);
            return new ExpansionContext(new_stack, Scope, Names, Env){Parent = this};
        }
    }
}