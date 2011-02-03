using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Conflux.Core.Configuration.Cuda.Codebase;
using Conflux.Runtime.Cuda.Api;
using Truesight.Decompiler;
using Truesight.Decompiler.Hir;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using Truesight.Decompiler.Hir.Core.Scopes;
using Truesight.Decompiler.Hir.Core.Symbols;
using Truesight.Decompiler.Hir.Traversal;
using Truesight.Decompiler.Hir.Traversal.Transformers;
using Truesight.Decompiler.Hir.TypeInference;
using Truesight.Parser;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Reflection.Generics;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Traits.Equivatable;
using CilNop = Truesight.Parser.Api.Ops.Nop;
using CilNew = Truesight.Parser.Api.Ops.New;
using CilThrow = Truesight.Parser.Api.Ops.Throw;
using Convert = Truesight.Decompiler.Hir.Core.Expressions.Convert;

namespace Conflux.Runtime.Cuda.Jit.Inliner
{
    internal class ExpressionExpander
    {
        private Expression Source { get; set; }
        private List<Node> Stmts { get; set; }
        private Expression Result { get; set; }

        private ExpansionContext Ctx { get; set; }
        private CudaConfig Config { [DebuggerNonUserCode] get { return Ctx.Config; } }
        private CudaCodebase Codebase { [DebuggerNonUserCode] get { return Config.Codebase; } }
        private Stack<MethodBase> Stack { [DebuggerNonUserCode] get { return Ctx.Stack; } }
        private Scope Scope { [DebuggerNonUserCode] get { return Ctx.Scope; } }
        private Dictionary<Sym, Expression> Env { [DebuggerNonUserCode] get { return Ctx.Env; } }

        [DebuggerNonUserCode] public static ExpandedExpression ExpandExpression(Expression source, ExpansionContext ctx) { var expander = new ExpressionExpander(source, ctx); return new ExpandedExpression(expander.Stmts, expander.Result); }
        [DebuggerNonUserCode] private ExpressionExpander(Expression source, ExpansionContext ctx)
        {
            Source = source;
            Ctx = ctx;
            Stmts = new List<Node>();
            Result = Expand(Source);
        }

        private Ref DeclareLocal(String prefix, Type type) { return DeclareLocal_Impl(GenerateName(prefix), type); }
        private void RemoveLocal(Ref @ref) { RemoveLocal(@ref.Sym.AssertCast<Local>()); }
        private void RemoveLocal(Local local) { Scope.Locals.Remove(local); }
        private Ref DeclareLocal_Impl(String name, Type type) { var local = new Local(name, type); Scope.Locals.Add(local); return new Ref(local); }
        private String GenerateName(String prefix) { return Ctx.Names.UniqueName(prefix); }

        private void Emit(Expression expr)
        {
            var inlined = Expand(expr);
            Stmts.Add(inlined);
        }

        private Expression Expand(Expression expr)
        {
            if (expr == null)
            {
                return null;
            }
            else if (expr is Addr)
            {
                var addr = (Addr)expr;
                var target = Expand(addr.Target);
                return new Addr(target);
            }
            else if (expr is Assign)
            {
                var ass = (Assign)expr;

                var prop = ass.InvokedProp();
                if (prop != null)
                {
                    return Expand(prop);
                }
                else
                {
                    var rhs = Expand(ass.Rhs);
                    var lhs = Expand(ass.Lhs);
                    return new Assign(lhs, rhs);
                }
            }
            else if (expr is Operator)
            {
                var op = (Operator)expr;
                if (op.OperatorType.IsAssign())
                {
                    var prop = op.Children.AssertFirst().InvokedProp();
                    if (prop != null)
                    {
                        return Expand(prop);
                    }
                    else
                    {
                        // todo. implement this with the use of SafeExpandOpAssign
                        var args = op.Args.Select(arg => Expand(arg));
                        return Operator.Create(op.OperatorType, args);
                    }
                }
                else
                {
                    var args = op.Args.Select(arg => Expand(arg));
                    return Operator.Create(op.OperatorType, args);
                }
            }
            else if (expr is Conditional)
            {
                var cond = (Conditional)expr;
                var test = Expand(cond.Test);
                var iftrue = Expand(cond.IfTrue);
                var iffalse = Expand(cond.IfFalse);
                return new Conditional(test, iftrue, iffalse);
            }
            else if (expr is Const)
            {
                // do nothing - nowhere to drill into
                return expr;
            }
            else if (expr is Convert)
            {
                var cvt = (Convert)expr;
                var source = Expand(cvt.Source);
                return new Convert(cvt.Type, source);
            }
            else if (expr is Deref)
            {
                var deref = (Deref)expr;
                var target = Expand(deref.Target);
                return new Deref(target);
            }
            else if (expr is Fld)
            {
                var fld = (Fld)expr;
                var @this = Expand(fld.This);
                return new Fld(fld.Field, @this);
            }
            else if (expr is Prop)
            {
                // basic investigations
                var prop = (Prop)expr;
                var is_instance = prop.Property.IsInstance();
                var parent = prop.Parent;
                var app = parent as Apply;
                var is_indexer = app != null && app.Callee == prop;
                if (is_indexer) parent = parent.Parent;

                // we have 5 different cases:
                // 1) foo.P;
                // 2) foo.P = bar;
                // 3) qux = (foo.P = bar);
                // 4) foo.P += bar;
                // 4') foo.P++;
                // 5) qux = (foo.P += bar);
                // 5') qux = (foo.P++);
                var ass = parent as Assign;
                var is_assigned = ass != null && (ass.Lhs == prop || ass.Lhs == app);
                var op = parent as Operator;
                var is_opassigned = op != null && op.OperatorType.IsAssign();
                is_assigned |= is_opassigned;
                var is_rhs_reused = is_assigned && parent.Parent is Expression;

                if (!is_assigned)
                {
                    var impl = prop.Property.GetGetMethod(true);
                    var this_args = is_instance ? prop.This.MkArray() : Seq.Empty<Expression>();
                    var indexer_args = is_indexer ? app.Args : Seq.Empty<Expression>();
                    var args = Seq.Concat(this_args, indexer_args).ToReadOnly();
                    var style = prop.InvokedAsVirtual ? InvocationStyle.Virtual : InvocationStyle.NonVirtual;
                    return Expand(new Eval(new Apply(new Lambda(impl, style), args)));
                }
                else
                {
                    // abstract away the root
                    // todo. implement this with the use of SafeExpandOpAssign
                    var root = prop.This;
                    if (is_opassigned && !root.IsLvalue())
                    {
                        var opassroot = DeclareLocal("$opassroot", prop.This.Type());
                        Emit(new Assign(opassroot, prop.This));
                        root = opassroot;
                    }

                    // abstract away the RHS
                    var rhs = null as Expression;
                    if (ass != null) rhs = ass.Rhs;
                    if (is_opassigned)
                    {
                        if (op.IsUnary()) rhs = new Const(1);
                        else if (op.IsBinary()) rhs = op.Children.AssertSecond().AssertCast<Expression>();
                        else throw AssertionHelper.Fail();
                    }

                    // abstract away the equivalence transform
                    Func<Expression> equiv = () =>
                    {
                        Func<Expression> equivGetter = () =>
                        {
                            var impl = prop.Property.GetGetMethod(true);
                            var this_args = is_instance ? root.MkArray() : Seq.Empty<Expression>();
                            var indexer_args = is_indexer ? app.Args : Seq.Empty<Expression>();
                            var args = Seq.Concat(this_args, indexer_args).ToReadOnly();
                            var style = prop.InvokedAsVirtual ? InvocationStyle.Virtual : InvocationStyle.NonVirtual;
                            return new Eval(new Apply(new Lambda(impl, style), args));
                        };

                        Func<Expression, Expression> equivSetter = assigned_value =>
                        {
                            var impl = prop.Property.GetSetMethod(true);
                            var this_args = is_instance ? root.MkArray() : Seq.Empty<Expression>();
                            var indexer_args = is_indexer ? app.Args : Seq.Empty<Expression>();
                            var args = Seq.Concat(this_args, indexer_args, assigned_value.MkArray()).ToReadOnly();
                            var style = prop.InvokedAsVirtual ? InvocationStyle.Virtual : InvocationStyle.NonVirtual;
                            return new Eval(new Apply(new Lambda(impl, style), args));
                        };

                        if (is_opassigned)
                        {
                            return equivSetter(Operator.Create(op.OperatorType, equivGetter(), rhs));
                        }
                        else
                        {
                            return equivSetter(rhs);
                        }
                    };

                    // final transform
                    if (is_rhs_reused)
                    {
                        var cached_rhs = DeclareLocal("$opassrhs", rhs.Type());
                        Emit(new Assign(cached_rhs, rhs));
                        rhs = cached_rhs;
                        Emit(equiv());
                        return cached_rhs;
                    }
                    else
                    {
                        return Expand(equiv());
                    }
                }
            }
            else if (expr is Ref)
            {
                var @ref = (Ref)expr;
                var env = Env.GetOrDefault(@ref.Sym);
                return Expand(env) ?? @ref;
            }
            else if (expr is SizeOf)
            {
                // do nothing - nowhere to drill into
                return expr;
            }
            else if (expr is TypeAs)
            {
                var typeAs = (TypeAs)expr;
                var target = Expand(typeAs.Target);
                return new TypeAs(typeAs.Type, target);
            }
            else if (expr is TypeIs)
            {
                var typeIs = (TypeIs)expr;
                var target = Expand(typeIs.Target);
                return new TypeAs(typeIs.Type, target);
            }
            else if (expr is Default)
            {
                // do nothing - nowhere to drill into
                return expr;
            }
            else if (expr is CollectionInit)
            {
                var ci = (CollectionInit)expr;
                if (ci.Elements.IsEmpty())
                {
                    return Expand(ci.Ctor);
                }
                else
                {
                    var ctor_coll = ci.InvokedMethod().AssertThat(mb => mb.IsConstructor());
                    var t_coll = ctor_coll.DeclaringType;

                    var l_coll = DeclareLocal("$", t_coll);
                    Emit(new Assign(l_coll, ci.Ctor));
                    ci.Elements.ForEach((el, i) =>
                    {
                        if (t_coll.IsArray)
                        {
                            (t_coll.GetArrayRank() == 1).AssertTrue();
                            var m_set = t_coll.ArraySetter().AssertNotNull();
                            Emit(new Eval(new Apply(new Lambda(m_set), l_coll, new Const(i), el)));
                        }
                        else
                        {
                            var m_add = t_coll.GetMethods(BF.All).AssertSingle(m => m.Name == "Add");
                            Emit(new Eval(new Apply(new Lambda(m_add), l_coll, el)));
                        }
                    });

                    return l_coll;
                }
            }
            else if (expr is ObjectInit)
            {
                var oi = (ObjectInit)expr;
                if (oi.Members.IsEmpty())
                {
                    return Expand(oi.Ctor);
                }
                else
                {
                    var ctor_obj = oi.InvokedMethod().AssertThat(mb => mb.IsConstructor());
                    var t_obj = ctor_obj.DeclaringType;

                    var l_obj = DeclareLocal("$", t_obj);
                    Emit(new Assign(l_obj, oi.Ctor));
                    foreach (var mi in oi.Members)
                    {
                        if (mi is FieldInfo)
                        {
                            var fi = mi as FieldInfo;
                            Emit(new Assign(new Fld(fi, l_obj), oi.MemberInits[mi]));
                        }
                        else if (mi is PropertyInfo)
                        {
                            var pi = mi as PropertyInfo;
                            // todo. what about virtuality?!
                            Emit(new Assign(new Prop(pi, l_obj), oi.MemberInits[mi]));
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
                    }

                    return l_obj;
                }
            }
            else if (expr is Eval)
            {
                var eval = (Eval)expr;
                var lam = eval.InvokedLambda();
                var m = eval.InvokedMethod();
                var child_ctx = Ctx.SpinOff(m);

                var status = Codebase.Classify(m);
                (status != MethodStatus.MustNotBeExecutedOnDevice).AssertTrue();
                var is_redirected = status == MethodStatus.IsRedirected;
                if (is_redirected)
                {
                    var redir = Codebase.Redirect(eval);
                    if (redir == null) return null;
                    else
                    {
                        var m_redir = redir.InvokedMethod();
                        if (m_redir.HasBody())
                        {
                            var raw_body = m_redir.ParseBody().Where(op => !(op is CilNop)).ToReadOnly();
                            if (raw_body.Count() == 2)
                            {
                                var first = raw_body.First() as CilNew;
                                var second = raw_body.Second() as CilThrow;

                                var tni_ctor = typeof(NotImplementedException).GetConstructor(Type.EmptyTypes);
                                if (first != null && first.Ctor == tni_ctor && second != null)
                                {
                                    throw AssertionHelper.Fail();
                                }
                            }
                        }

                        return Expand(redir);
                    }
                }
                else
                {
                    var needsExpansion = status == MethodStatus.CanBeExecutedOnDevice;
                    var doesntNeedExpansion = status == MethodStatus.HasSpecialSemantics;
                    (needsExpansion ^ doesntNeedExpansion).AssertTrue();
                    if (needsExpansion)
                    {
                        // todo. think about what we can do here
                        (lam.InvocationStyle == InvocationStyle.Virtual).AssertFalse();
                        m.DeclaringType.IsInterface.AssertFalse();
                    }

                    var md = m.Decompile();
                    needsExpansion.AssertImplies(m.HasBody());
                    var p_ret = Seq.Empty<ParameterInfo>();
                    var mi = m as MethodInfo;
                    if (mi != null) p_ret = mi.ReturnParameter.MkArray();
                    m.GetParameters().Concat(p_ret).ForEach(pi => pi.IsOptional.AssertFalse());

                    var args = eval.InvocationArgs();
                    var args_include_this = !lam.InvokedAsCtor && m.IsInstance();
                    var @params = m.GetParameters().AsEnumerable();
                    var p_fakethis = null as ParameterInfo;
                    if (args_include_this) @params = p_fakethis.Concat(@params).ToReadOnly();

                    var l_args = @params.Zip(args, (p, actual_arg) =>
                    {
                        var p_sig = p == null ? md.Sig.Params.First() : md.Sig.Params.AssertSingle(p1 => p1.Metadata == p);
                        var p_type = p_sig.Type;

                        Func<Expression, Expression> expand_arg = null;
                        expand_arg = arg =>
                        {
                            Func<Expression, String, Expression> default_expand1 = (e, postfix) =>
                            {
                                var prefix = (m.IsConstructor() ? m.DeclaringType.Name : m.Name).ToLower();
                                var name = p_sig.Name + (postfix == null ? null : ("_" + postfix.ToLower()));
                                var full_name = String.Format("${0}_{1}", prefix, name);

                                var l_stub = DeclareLocal(full_name, p_type);
                                Emit(new Assign(l_stub, Expand(arg)));
                                return l_stub;
                            };
                            Func<Expression, Expression> default_expand = e => default_expand1(e, null);

                            if (doesntNeedExpansion)
                            {
                                if (p_type.IsArray && p.IsVarargs())
                                {
                                    var ctor = arg.InvokedCtor();
                                    if (ctor != null && ctor.DeclaringType.IsArray)
                                    {
                                        var arg_eval = arg as Eval;
                                        if (arg_eval != null)
                                        {
                                            var rank = ctor.DeclaringType.GetArrayRank();
                                            if (rank == 1)
                                            {
                                                var sole_arg = arg.InvocationArgs().AssertSingle() as Const;
                                                if (sole_arg != null &&
                                                    sole_arg.Value is int && (int)sole_arg.Value == 0)
                                                {
                                                    return arg;
                                                }
                                            }
                                        }

                                        var arg_ci = arg as CollectionInit;
                                        if (arg_ci != null)
                                        {
                                            p_type.IsArray().AssertTrue();
                                            (p_type.GetArrayRank() == 1).AssertTrue();

                                            try
                                            {
                                                p_type = p_type.GetElementType();
                                                var els = arg_ci.Elements.Select(expand_arg).ToReadOnly();
                                                return new CollectionInit(arg_ci.Ctor, els);
                                            }
                                            finally 
                                            {
                                                p_type = p_type.MakeArrayType();
                                            }
                                        }
                                    }
                                }

                                var needs_expansion = arg.Family().Any(c => 
                                    c is Eval || c is Apply || c is Lambda || c is Prop || c is CollectionInit || c is ObjectInit);
                                if (!needs_expansion) return arg;
                                else
                                {
                                    // todo. the stuff below works incorrectly in general case
                                    // since it might disrupt evaluation order of parameters
                                    //
                                    // however for now I trade off introducing a potential bug
                                    // for the ease of debugging and looking at traces

                                    var old_stmtc = Stmts.Count();
                                    var expanded = default_expand(arg).AssertCast<Ref>();
                                    var new_stmtc = Stmts.Count();
                                    (new_stmtc > old_stmtc).AssertTrue();

                                    // todo. fix possible semantic disruption at the next line
                                    if (new_stmtc - old_stmtc > 1) return expanded;
                                    else
                                    {
                                        var ass = Stmts.Last().AssertCast<Assign>();
                                        ass.Lhs.Equiv(expanded).AssertTrue();
                                        Stmts.RemoveLast();
                                        RemoveLocal(expanded);
                                        return ass.Rhs;
                                    }
                                }
                            }
                            else
                            {
                                var p_reads = md.Body.Family().OfType<Ref>().Where(r => r.Sym == p_sig.Sym).ToReadOnly();
                                var p_asses = md.Body.Family().OfType<Assign>().Select(ass =>
                                {
                                    var r_lhs = ass.Lhs as Ref;
                                    var is_write = r_lhs != null && r_lhs.Sym == p_sig.Sym;
                                    return is_write ? ass : null;
                                }).Where(ass => ass != null).ToReadOnly();
                                var p_byref = md.Body.Family().OfType<Apply>().Select(app =>
                                {
                                    var passes_byref = app.ArgsInfo.Zip((e, pi) =>
                                    {
                                        var e_ref = e as Ref;
                                        var is_read = e_ref != null && e_ref.Sym == p_sig.Sym;
                                        var is_byref = pi.Type.IsByRef;
                                        return is_read && is_byref;
                                    }).Any();
                                    return passes_byref ? app : null;
                                }).Where(app => app != null).ToReadOnly();
                                var p_writes = Seq.Concat(p_asses.Cast<Expression>(), p_byref.Cast<Expression>()).ToReadOnly();
                                var p_usages = Seq.Concat(p_reads.Cast<Expression>(), p_writes.Cast<Expression>()).ToReadOnly();

                                // todo. below we might disrupt evaluation order
                                // by totally inlining an arg expression if it has a single usage
                                // strictly speaking, before doing that
                                // we need perform additional checks that eval-order is preserved
                                //
                                // note. this semi-correct solution is introduced 
                                // solely for the convenience of the back-end
                                // and for the convenience of reading the resulting traces

                                var passed_by_ref = p == null || p.PassedByRef();
                                if (passed_by_ref)
                                {
                                    var @ref = arg as Ref;
                                    if (@ref != null) return arg;

                                    var fld = arg as Fld;
                                    if (fld != null)
                                    {
                                        fld.Field.IsInstance().AssertTrue();

                                        // todo. fix possible semantic disruption at the next line
                                        if (p_usages.Count() <= 1) return arg;
                                        else
                                        {
                                            var root = fld.This;
                                            var is_atom = root is Ref || root is Const;
                                            if (is_atom) return arg;
                                            else
                                            {
                                                var root_expanded = default_expand1(root, "root");
                                                return new Fld(fld.Field, root_expanded);
                                            }
                                        }
                                    }

                                    var eval1 = arg as Eval;
                                    if (eval1 != null)
                                    {
                                        var m1 = eval1.InvokedMethod();
                                        m1.IsArrayGetter().AssertTrue();
                                        var args1 = eval1.InvocationArgs();
                                        var lam1 = eval1.InvokedLambda();

                                        var r_ee = eval1.Expand(Ctx.SpinOff(m1));
                                        r_ee.Stmts.ForEach(Stmts.Add);
                                        var ee = r_ee.Result.AssertCast<Eval>().AssertNotNull();

                                        // todo. fix possible semantic disruption at the next line
                                        if (p_usages.Count() <= 1) return ee;
                                        else
                                        {
                                            var root = ee.Callee.Callee;
                                            Func<Expression, bool> is_atom = e => e is Ref || e is Const;
                                            if (is_atom(root) && ee.Callee.Args.All(is_atom)) return ee;
                                            else
                                            {
                                                var root_expanded = default_expand1(root, "root");
                                                return new Eval(new Apply(new Lambda(m1, lam1.InvocationStyle), root_expanded.Concat(args1)));
                                            }
                                        }
                                    }

                                    // arg isn't an lvalue, so it can't be passed by reference
                                    throw AssertionHelper.Fail();
                                }
                                else
                                {
                                    if (p_writes.IsEmpty())
                                    {
                                        // todo. fix possible semantic disruption at the next line
                                        if (p_usages.Count() <= 1) return arg;
                                        else
                                        {
                                            var is_atom = arg is Ref || arg is Const;
                                            if (is_atom)
                                            {
                                                var is_value_type = p_type.IsValueType;
                                                var is_primitive = p_type.IsPrimitive;
                                                var needs_copying = is_value_type && !is_primitive;
                                                return needs_copying ? default_expand(arg) : arg;
                                            }
                                            else
                                            {
                                                return default_expand(arg);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return default_expand(arg);
                                    }
                                }
                            }
                        };

                        var preprocessed_arg = actual_arg.Transform((Ref @ref) =>
                        {
                            var env = Env.GetOrDefault(@ref.Sym);
                            return env ?? @ref;
                        }).AssertCast<Expression>();
                        var expanded_arg = expand_arg(preprocessed_arg);
                        expanded_arg = expanded_arg.Transform((Ref @ref) =>
                        {
                            var env = Env.GetOrDefault(@ref.Sym);
                            return env ?? @ref;
                        }).AssertCast<Expression>();

                        if (needsExpansion) child_ctx.Env.Add(p_sig.Sym, expanded_arg);
                        return expanded_arg;
                    }).ToReadOnly();

                    if (needsExpansion)
                    {
                        var env_locals = lam.Body.LocalsRecursive().ToDictionary(l => l as Sym, l =>
                        {
                            var frames = Stack.Reverse().Skip(1).Concat(m);
                            var qualifier = frames.Select(sf => sf.Name.ToLower()).StringJoin("_");
                            var full_name = String.Format("${0}_{1}", qualifier, l.Name);
                            return DeclareLocal(full_name, l.Type) as Expression;
                        }).ToReadOnly();
                        child_ctx.Env.AddElements(env_locals);

                        Action<Block> import_locals = blk =>
                        {
                            foreach (var local in blk.Locals.ToArray())
                            {
                                var env_ref = env_locals.GetOrDefault(local) as Ref;
                                var env = env_ref == null ? null : (Local)env_ref.Sym;
                                if (env != null)
                                {
                                    blk.Locals.Remove(local);
                                    blk.Locals.Add(env);
                                }
                                else
                                {
                                    Scope.Locals.Add(local);
                                }
                            }

                            // todo. also import locals from embedded blocks
                            // here we need to take inlined stuff into account
                        };

                        if (lam.InvokedAsCtor)
                        {
                            var l_this = DeclareLocal(String.Format("${0}_this", m.DeclaringType.Name.ToLower()), m.DeclaringType);
                            child_ctx.Env.Add(lam.Sig.Syms[0], l_this);

                            var malloc = typeof(Ctm).GetMethod("Malloc", new []{typeof(Type)}).AssertNotNull();
                            var malloc_type = new Const(m.DeclaringType);
                            Emit(new Assign(l_this, new Eval(new Apply(new Lambda(malloc), malloc_type))));

                            var body = lam.Body.Expand(child_ctx);
                            import_locals(body);

                            if (body.IsEmpty())
                            {
                                var last_stmt = Stmts.Last() as Assign;
                                var last_invoked = last_stmt == null ? null : last_stmt.Rhs.InvokedMethod();
                                (last_invoked == malloc).AssertTrue();

                                RemoveLocal(l_this);
                                Stmts.RemoveLast();
                                return last_stmt.Rhs;
                            }
                            else
                            {
                                body.ForEach(Stmts.Add);
                                return l_this;
                            }
                        }
                        else if (m.Ret() == typeof(void))
                        {
                            var body = lam.Body.Expand(child_ctx).AssertNotEmpty();
                            import_locals(body);

                            if (body.IsNotEmpty()) body.ForEach(Stmts.Add);
                            return null;
                        }
                        else
                        {
                            (m.Ret().IsByRef || m.Ret().IsPointer).AssertFalse();

                            var name = String.Format("${0}_ret", m.Name.ToLower());
                            var l_ret = DeclareLocal(name, m.Ret());
                            child_ctx.Ret = l_ret;

                            var body = lam.Body.Expand(child_ctx).AssertNotEmpty();
                            import_locals(body);

                            var body_last = body.LastOrDefault();
                            if (body_last is Label)
                            {
                                body.ForEach(Stmts.Add);
                                return l_ret;
                            }
                            else
                            {
                                var ass = body.AssertLast().AssertCast<Assign>();
                                ass.Lhs.Equiv(l_ret).AssertTrue();

                                RemoveLocal(l_ret);
                                body.SkipLast(1).ForEach(Stmts.Add);
                                return ass.Rhs;
                            }
                        }
                    }
                    else
                    {
                        return new Eval(new Apply(lam, l_args.Cast<Expression>()));
                    }
                }
            }
            else
            {
                var app = expr as Apply;
                var app_prop = app == null ? null : app.Callee as Prop;
                if (app_prop != null) return Expand(app_prop);

                // todo. also support indirect calls and partial applications
                // i.e. process cases when Apply/Lambda nodes ain't wrapped in an Eval
                throw AssertionHelper.Fail();
            }
        }
    }
}