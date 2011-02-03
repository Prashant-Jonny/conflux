using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Functional;
using XenoGears.Assertions;
using Truesight.Decompiler.Hir.Traversal;
using XenoGears.Functional;

namespace Conflux.Core.Configuration.Cuda
{
    [DebuggerNonUserCode]
    public class Codebase
    {
        private readonly List<Func<MethodBase, bool>> _optIn = new List<Func<MethodBase, bool>>();
        public Codebase OptIn(params Type[] ts) { return OptIn((IEnumerable<Type>)ts); }
        public Codebase OptIn(IEnumerable<Type> ts) { return OptIn(t => ts.Contains(t)); }
        public Codebase OptIn(Func<Type, bool> filter) { return OptIn((MethodBase mb) => filter(mb.DeclaringType)); }
        public Codebase OptIn(params MethodBase[] mbs) { return OptIn((IEnumerable<MethodBase>)mbs); }
        public Codebase OptIn(IEnumerable<MethodBase> mbs) { return OptIn(mb => mbs.Contains(mb)); }
        public Codebase OptIn(Func<MethodBase, bool> filter) { _optIn.Add(filter.AssertNotNull()); return this; }

        private readonly List<Func<MethodBase, bool>> _optOut = new List<Func<MethodBase, bool>>();
        public Codebase OptOut(params Type[] ts) { return OptOut((IEnumerable<Type>)ts); }
        public Codebase OptOut(IEnumerable<Type> ts) { return OptOut(t => ts.Contains(t)); }
        public Codebase OptOut(Func<Type, bool> filter) { return OptOut((MethodBase mb) => filter(mb.DeclaringType)); }
        public Codebase OptOut(params MethodBase[] mbs) { return OptOut((IEnumerable<MethodBase>)mbs); }
        public Codebase OptOut(IEnumerable<MethodBase> mbs) { return OptOut(mb => mbs.Contains(mb)); }
        public Codebase OptOut(Func<MethodBase, bool> filter) { _optOut.Add(filter.AssertNotNull()); return this; }

        private readonly List<Func<MethodBase, bool>> _special = new List<Func<MethodBase, bool>>();
        public Codebase Special(params Type[] ts) { return Special((IEnumerable<Type>)ts); }
        public Codebase Special(IEnumerable<Type> ts) { return Special(t => ts.Contains(t)); }
        public Codebase Special(Func<Type, bool> filter) { return Special((MethodBase mb) => filter(mb.DeclaringType)); }
        public Codebase Special(params MethodBase[] mbs) { return Special((IEnumerable<MethodBase>)mbs); }
        public Codebase Special(IEnumerable<MethodBase> mbs) { return Special(mb => mbs.Contains(mb)); }
        public Codebase Special(Func<MethodBase, bool> filter) { _special.Add(filter.AssertNotNull()); return this; }

        private readonly Dictionary<Func<MethodBase, bool>, Tuple<Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase>, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>>>> _redirects = new Dictionary<Func<MethodBase, bool>, Tuple<Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase>, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>>>>();
        public Codebase Redirect(IEnumerable<Type> ts, Func<MethodBase, MethodBase> map_m) { return Redirect(t => ts.Contains(t), map_m); }
        public Codebase Redirect(IEnumerable<Type> ts, Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase> map_m, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>> map_args) { return Redirect(t => ts.Contains(t), map_m, map_args); }
        public Codebase Redirect(Func<Type, bool> filter, Func<MethodBase, MethodBase> map_m) { return Redirect((MethodBase mb) => filter(mb.DeclaringType), map_m); }
        public Codebase Redirect(Func<Type, bool> filter, Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase> map_m, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>> map_args) { return Redirect((MethodBase mb) => filter(mb.DeclaringType), map_m, map_args); }
        public Codebase Redirect(IEnumerable<MethodBase> mbs, Func<MethodBase, MethodBase> map_m) { return Redirect(mb => mbs.Contains(mb), map_m); }
        public Codebase Redirect(IEnumerable<MethodBase> mbs, Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase> map_m, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>> map_args) { return Redirect(mb => mbs.Contains(mb), map_m, map_args); }
        public Codebase Redirect(Func<MethodBase, bool> filter, Func<MethodBase, MethodBase> map_m) { return Redirect(filter, (m, _) => map_m(m), (_, args) => args); }
        public Codebase Redirect(Func<MethodBase, bool> filter, Func<MethodBase, ReadOnlyCollection<Expression>, MethodBase> map_m, Func<MethodBase, ReadOnlyCollection<Expression>, IEnumerable<Expression>> map_args) { _redirects.Add(filter.AssertNotNull(), Tuple.New(map_m.AssertNotNull(), map_args.AssertNotNull())); return this; }
        public Codebase Ignore(params Type[] ts) { return Ignore((IEnumerable<Type>)ts); }
        public Codebase Ignore(IEnumerable<Type> ts) { return Ignore(t => ts.Contains(t)); }
        public Codebase Ignore(Func<Type, bool> filter) { return Ignore((MethodBase mb) => filter(mb.DeclaringType)); }
        public Codebase Ignore(params MethodBase[] mbs) { return Ignore((IEnumerable<MethodBase>)mbs); }
        public Codebase Ignore(IEnumerable<MethodBase> mbs) { return Ignore(mb => mbs.Contains(mb)); }
        public Codebase Ignore(Func<MethodBase, bool> filter) { return Redirect(filter, mb => null); }

        public MethodStatus Classify(MethodBase mb)
        {
            if (_redirects.Any(f => f.Key(mb))) return MethodStatus.IsRedirected;
            else if (_special.Any(f => f(mb))) return MethodStatus.HasSpecialSemantics;
            else
            {
                var optIn = _optIn.Any(f => f(mb));
                var optOut = _optOut.Any(f => f(mb));
                var allowed = optIn && !optOut;
                return allowed ? MethodStatus.CanBeExecutedOnDevice : MethodStatus.MustNotBeExecutedOnDevice;
            }
        }

        public Eval Redirect(Eval eval)
        {
            var m = eval.InvokedMethod();
            var a = eval.InvocationArgs().ToReadOnly();

            var redirectors = _redirects.Select(kvp => kvp.Key(m) ? kvp.Value : null).Where(f => f != null);
            var f_mredir = redirectors.AssertSingle().Item1;
            var m_redir = f_mredir(m, a);
            var f_aredir = redirectors.AssertSingle().Item2;
            var a_redir = f_aredir(m, a);

            if (m_redir == null || a_redir == null) return null;
            // todo. this doesn't preserve virtuality of the call!
            return new Eval(new Apply(new Lambda(m_redir), a_redir));
        }
    }
}