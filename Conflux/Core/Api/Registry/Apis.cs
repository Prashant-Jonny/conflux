using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Conflux.Runtime.Cuda.Api.Registry;
using XenoGears.Collections.Dictionaries;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Assertions;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Reflection.Generics;

namespace Conflux.Core.Api.Registry
{
    [DebuggerNonUserCode]
    internal static class Apis
    {
        private static readonly Object _initLock = new Object();
        static Apis()
        {
            lock (_initLock)
            {
                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                var types = asm.GetTypes().Where(t => t.HasAttr<ApiAttribute>()).ToHashSet();
                Ifaces = types.Where(t => t.IsInterface).ToHashSet();
                Statics = types.Where(t => t.IsStatic()).ToHashSet();
                (types.Count() == Ifaces.Count() + Statics.Count()).AssertTrue();

                Main = Ifaces.SelectMany(t => t.GetMethods(BF.All)).Cast<MethodBase>().ToHashSet();
                Mirrors = Ifaces.SelectMany(t => t.GetMethods(BF.All)).Cast<MethodBase>().ToHashSet();
            }
        }

        public static HashSet<Type> Ifaces { get; private set; }
        public static HashSet<Type> Statics { get; private set; }
        public static bool IsApi(this Type t) { return t.IsMainApi() || t.IsMirrorApi(); }
        public static bool IsMainApi(this Type t) { return Ifaces.Contains(t); }
        public static bool IsMirrorApi(this Type t) { return Statics.Contains(t); }

        public static HashSet<MethodBase> Main { get; private set; }
        public static HashSet<MethodBase> Mirrors { get; private set; }
        public static bool IsApi(this MethodBase m) { return m.IsMainApi() || m.IsMirrorApi(); }
        public static bool IsMainApi(this MethodBase m) { return m.Api() != null; }
        public static bool IsMirrorApi(this MethodBase m) { return Mirrors.Contains(m); }
        public static bool IsApi(this PropertyInfo p) { return p.IsMainApi() || p.IsMirrorApi(); }
        public static bool IsMainApi(this PropertyInfo p) { return p != null && (p.GetGetMethod(true).IsMainApi() || p.GetSetMethod(true).IsMainApi()); }
        public static bool IsMirrorApi(this PropertyInfo p) { return p != null && (p.GetGetMethod(true).IsMirrorApi() || p.GetSetMethod(true).IsMirrorApi()); }

        private static readonly Dictionary<MethodBase, MethodBase> _mapiCache = new Dictionary<MethodBase, MethodBase>();
        public static MethodBase Api(this MethodBase m)
        {
            if (m == null) return null;
            else return _mapiCache.GetOrCreate(m, () =>
            {
                if (m.IsImpl()) return null;
                else if (m is ConstructorInfo) return null;
                else
                {
                    foreach (var hm in m.Hierarchy())
                    {
                        if (Main.Contains(hm)) return hm;
                        if (Mirrors.Contains(hm)) return hm;

                        var t = hm.DeclaringType;
                        var alts = t.GetMethods(BF.AllInstance)
                            .Where(hm1 =>
                            {
                                var implOf = hm1.ExplicitImplOf();
                                return implOf != null && hm.Name == implOf.Name;
                            })
                            .Where(hm1 => Seq.Equal(hm1.Params(), hm.Params()));
                        var alt = alts.SingleOrDefault2().ExplicitImplOf();
                        if (Main.Contains(alt)) return alt;
                        if (Mirrors.Contains(alt)) return alt;
                    }

                    return null;
                }
            });
        }

        private static readonly Dictionary<PropertyInfo, PropertyInfo> _papiCache = new Dictionary<PropertyInfo, PropertyInfo>();
        public static PropertyInfo Api(this PropertyInfo p)
        {
            if (p == null) return null;
            else return _papiCache.GetOrCreate(p, () =>
            {
                var getter_api = p.GetGetMethod(true).Api();
                var setter_api = p.GetGetMethod(true).Api();
                var m_api = getter_api ?? setter_api;
                if (m_api == null) return null;

                var p_api = m_api.EnclosingProperty();
                return p_api.AssertNotNull();
            });
        }

        private static readonly Dictionary<MethodBase, MethodBase> _mmapiCache = new Dictionary<MethodBase, MethodBase>();
        public static MethodBase MirrorApi(this MethodBase m)
        {
            if (m == null) return null;
            else return _mmapiCache.GetOrCreate(m, () =>
            {
                m = m.Api();

                if (m == null) return null;
                else
                {
                    var t = m.DeclaringType;
                    var iface = t.IsInterface;
                    var n_mirror = iface ? t.Name.Substring(1) : "I" + t.Namespace;
                    var asm = MethodBase.GetCurrentMethod().DeclaringType.Assembly;
                    var t_mirror = asm.GetType(t.Namespace + "." + n_mirror, true);

                    var m_mirror = t_mirror.GetMethods(BF.All).AssertSingle(mm =>
                        m.Name == mm.Name && Seq.Equal(m.Params(), mm.Params()) && m.Ret() == mm.Ret());
                    (m.IsStatic() ^ m_mirror.IsStatic()).AssertTrue();

                    return m_mirror;
                }
            });
        }

        private static readonly Dictionary<PropertyInfo, PropertyInfo> _pmapiCache = new Dictionary<PropertyInfo, PropertyInfo>();
        public static PropertyInfo MirrorApi(this PropertyInfo p)
        {
            if (p == null) return null;
            else return _pmapiCache.GetOrCreate(p, () =>
            {
                var getter_api = p.GetGetMethod(true).MirrorApi();
                var setter_api = p.GetGetMethod(true).MirrorApi();
                var m_api = getter_api ?? setter_api;
                if (m_api == null) return null;

                var p_api = m_api.EnclosingProperty();
                return p_api.AssertNotNull();
            });
        }

        public static HashSet<MethodBase> ApiIfaces(this Type t)
        {
            return t.ApiIfacesToImpls().Keys.ToHashSet();
        }

        public static HashSet<MethodBase> ApiImpls(this Type t)
        {
            return t.ApiIfacesToImpls().Values.ToHashSet();
        }

        private static readonly Dictionary<Type, ReadOnlyDictionary<MethodBase, MethodBase>> _aim2ifCache = new Dictionary<Type, ReadOnlyDictionary<MethodBase, MethodBase>>();
        public static ReadOnlyDictionary<MethodBase, MethodBase> ApiImplsToIfaces(this Type t)
        {
            if (t == null) return null;
            else return _aim2ifCache.GetOrCreate(t, () => t.ApiIfacesToImpls().ToDictionary(kvp => kvp.Value, kvp => kvp.Key).ToReadOnly());
        }

        private static readonly Dictionary<Type, ReadOnlyDictionary<MethodBase, MethodBase>> _aif2imCache = new Dictionary<Type, ReadOnlyDictionary<MethodBase, MethodBase>>();
        public static ReadOnlyDictionary<MethodBase, MethodBase> ApiIfacesToImpls(this Type t)
        {
            if (t == null) return null;
            else return _aif2imCache.GetOrCreate(t, () =>
            {
                var ifaces2impls = t.MapInterfacesToImpls(Ifaces).ToDictionary();
                ifaces2impls.Keys.ForEach(iface =>
                {
                    var impl = ifaces2impls[iface];
                    if (impl.IsExplicitImpl())
                    {
                        var t_impl = impl.DeclaringType;
                        var redirs = t_impl.GetMethods(BF.AllInstance)
                            .Where(m => m.Name == iface.Name)
                            .Where(m => Seq.Equal(m.Params(), iface.Params()));
                        var redir = redirs.SingleOrDefault2();
                        if (redir != null)
                        {
                            ifaces2impls[iface] = redir;
                        }
                    }
                });

                return ifaces2impls.ToReadOnly();
            });
        }
    }
}