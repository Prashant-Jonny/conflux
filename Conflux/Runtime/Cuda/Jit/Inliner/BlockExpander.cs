using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Scopes;
using Truesight.Decompiler.Hir.Core.Symbols;
using Truesight.Decompiler.Hir.Traversal;
using XenoGears.Functional;
using XenoGears.Assertions;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    internal class BlockExpander
    {
        private Block Source { get; set; }
        private Block Stmts { get; set; }

        private ExpansionContext Ctx { get; set; }
        private Stack<MethodBase> Stack { [DebuggerNonUserCode] get { return Ctx.Stack; } }
        private Scope Scope { [DebuggerNonUserCode] get { return Ctx.Scope; } }
        private Ref Ret { [DebuggerNonUserCode] get { return Ctx.Ret; } }
        private Label RetLabel { [DebuggerNonUserCode] get { return Ctx.RetLabel; } [DebuggerNonUserCode] set { Ctx.RetLabel = value; } }

        [DebuggerNonUserCode] public static Block ExpandBlock(Block source, ExpansionContext ctx) { return new BlockExpander(source, ctx).Stmts; }
        [DebuggerNonUserCode] public BlockExpander(Block source, ExpansionContext ctx)
        {
            Source = source;
            Ctx = ctx;

            Stmts = new Block();
            ctx.Scope = Stmts;
            var cloned_locals = source.Locals.Select(l => l.DeepClone());
            cloned_locals.ForEach(local => ctx.Scope.Locals.Add(local));

            var is_root = Ctx.Parent == null || (Ctx.Parent.Stack.Count() != Ctx.Stack.Count());
            if (is_root) RetLabel = new Label();
            else RetLabel.AssertNotNull();

            source.ForEach(Expand);
            if (is_root && Stmts.LastOrDefault() is Goto) Stmts.RemoveLast();
            var gotos = Stmts.Family().OfType<Goto>().Where(@goto => @goto.LabelId == RetLabel.Id).ToReadOnly();
            if (is_root && gotos.IsNotEmpty()) Stmts.Add(RetLabel);
        }

        private Ref DeclareLocal(String prefix, Type type) { return DeclareLocal_Impl(GenerateName(prefix), type); }
        private Ref DeclareLocal_Impl(String name, Type type) { var local = new Local(name, type); Scope.Locals.Add(local); return new Ref(local); }
        private String GenerateName(String prefix) { return Ctx.Names.UniqueName(prefix); }

        private void Expand(Node node)
        {
            if (node is Expression)
            {
                var expr = (Expression)node;
                var inlined = expr.Expand(Ctx);
                inlined.Stmts.ForEach(Stmts.Add);
                if (inlined.Result != null) Stmts.Add(inlined.Result);
            }
            else if (node is Block)
            {
                var block = (Block)node;
                Stmts.Add(block.Expand(Ctx.SpinOff()));
            }
            else if (node is Break)
            {
                Stmts.Add(node);
            }
            else if (node is Continue)
            {
                Stmts.Add(node);
            }
            else if (node is Goto)
            {
                Stmts.Add(node);
            }
            else if (node is Label)
            {
                Stmts.Add(node);
            }
            else if (node is If)
            {
                var @if = (If)node;

                var test = @if.Test.Expand(Ctx);
                test.Stmts.ForEach(Stmts.Add);
                test.Result.AssertNotNull();

                var if_true = @if.IfTrue.Expand(Ctx.SpinOff());
                var if_false = @if.IfFalse.Expand(Ctx.SpinOff());
                var expanded = new If(test.Result, if_true, if_false);
                Stmts.Add(expanded);
            }
            else if (node is Loop)
            {
                var loop = (Loop)node;

                var test = loop.Test.Expand(Ctx);
                test.Result.AssertNotNull();
                var init = loop.Init.Expand(Ctx.SpinOff());
                var iter = loop.Iter.Expand(Ctx.SpinOff());
                var body = loop.Body.Expand(Ctx.SpinOff());

                var prepend_test = loop.IsWhileDo && test.Stmts.IsNotEmpty();
                if (init.IsNotEmpty() && prepend_test) { Stmts.Add(init); init = new Block(); }
                if (prepend_test) test.Stmts.ForEach(Stmts.Add);
                test.Stmts.ForEach(iter.Add);

                var xloop = new Loop(test.Result, body, loop.IsWhileDo){Init = init, Iter = iter};
                var cloned_locals = loop.Locals.Select(l => l.DeepClone());
                cloned_locals.ForEach(local => xloop.Locals.Add(local));

                Stmts.Add(xloop);
            }
            else if (node is Return)
            {
                var ret = (Return)node;
                (ret.Value == null).AssertEquiv(Ret == null);
                if (ret.Value != null) Expand(new Assign(Ret, ret.Value));
                Stmts.Add(new Goto(RetLabel));
            }
            else if (node is Try || node is Clause || node is Throw ||
                node is Using || node is Iter)
            {
                // todo. implement support for non-linear control flow
                // this is only possible when we fully implement decompilation of tries
                // until now I leave this marked as "to be implemented"
                throw AssertionHelper.Fail();
            }
            else
            {
                throw AssertionHelper.Fail();
            }
        }
    }
}