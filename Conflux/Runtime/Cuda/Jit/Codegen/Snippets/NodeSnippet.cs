using System;
using System.Linq;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.TypeInference;
using XenoGears.Assertions;
using Truesight.Decompiler.Hir.Traversal;
using XenoGears.Reflection.Generics;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Snippets
{
    internal class NodeSnippet : Snippet
    {
        public Node Node { get; private set; }
        public NodeSnippet(Node node) { Node = node.AssertNotNull(); }

        public override int In
        {
            get
            {
                if (!(Node is Expression)) return 0;
                else
                {
                    var eval = Node as Eval;
                    if (eval != null)
                    {
                        var args = eval.InvocationArgs();
                        return args.Count();
                    }
                    else
                    {
                        return Node.Children.Count();
                    }
                }
            }
        }

        public override int Out
        {
            get
            {
                if (!(Node is Expression)) return 0;
                else
                {
                    var eval = Node as Eval;
                    if (eval != null)
                    {
                        var m = eval.InvokedMethod();
                        var returns_smth = m.Ret() != typeof(void) || eval.InvokedAsCtor();
                        return returns_smth ? 1 : 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }

        public override Type Type
        {
            get
            {
                return Node.Type();
            }
        }
    }
}