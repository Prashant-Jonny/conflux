using System;
using System.Linq;
using System.Reflection;
using Libptx.Common.Annotations.Quanta;
using Libptx.Common.Types;
using Libptx.Instructions;
using Libptx.Reflection;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Reflection.Shortcuts;
using XenoGears.Reflection.Attributes;
using XenoGears.Strings;
using Type=System.Type;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Snippets
{
    internal class PtxopSnippet : Snippet
    {
        public ptxop Ptxop { get; private set; }
        public PtxopSig Sig { get; set; }

        public PtxopSnippet(ptxop ptxop)
        {
            Ptxop = ptxop;
        }

        public PtxopSnippet(String opcode)
        {
            var splits = opcode.Split('.');
            var core = splits.AssertFirst();
            var s_affixes = splits.Skip(1).ToReadOnly();

            Ptxops.All.AssertSingle(t1 =>
            {
                if (!core.StartsWith(t1.Ptxopcode())) return false;
                else
                {
                    var ptxop = t1.CreateInstance().AssertCast<ptxop>();

                    var s_mod = core.Slice(t1.Ptxopcode().Length);
                    if (s_mod.IsNotEmpty())
                    {
                        var p_mods = t1.GetProperties(BF.PublicInstance).Where(p => p.HasAttr<ModAttribute>()).ToReadOnly();
                        var p_mod = p_mods.SingleOrDefault2(p => p.Signature() == s_mod);
                        if (p_mod == null) return false;
                        p_mod.SetValue(ptxop, true, null);
                    }

                    if (s_affixes.IsNotEmpty())
                    {
                        Func<PropertyInfo, String, Object> parse = (p, s1) =>
                        {
                            var t = p.PropertyType;
                            if (t.IsEnum)
                            {
                                var values = Enum.GetValues(t).Cast<Object>();
                                var value = values.SingleOrDefault(v => v.Signature() == s1);
                                return value;
                            }
                            else if (t == typeof(Libptx.Common.Types.Type))
                            {
                                var values = Enum.GetValues(typeof(TypeName)).Cast<TypeName>();
                                var value = values.SingleOrDefault(v => v.Signature() == s1);
                                return (Libptx.Common.Types.Type)value;
                            }
                            else
                            {
                                return null;
                            }
                        };

                        var p_affixes = t1.GetProperties(BF.PublicInstance).Where(p => p.HasAttr<AffixAttribute>()).ToDictionary(p => p.Signature(), p => p);
                        var failed = false;
                        s_affixes.ForEach(s_affix =>
                        {
                            if (p_affixes.ContainsKey(s_affix))
                            {
                                var p_affix = p_affixes[s_affix];
                                p_affix.SetValue(ptxop, true, null);
                            }
                            else
                            {
                                PropertyInfo p_affix = null;
                                Object o_affix = null;
                                p_affixes.Values.ForEach(p =>
                                {
                                    var o = parse(p, s_affix);
                                    if (o != null)
                                    {
                                        if (p_affix == null)
                                        {
                                            p_affix = p;
                                            o_affix = o;
                                        }
                                        else
                                        {
                                            failed = true;
                                            return;
                                        }
                                    }
                                });

                                if (p_affix != null && o_affix != null)
                                {
                                    p_affix.SetValue(ptxop, o_affix, null);
                                }
                                else
                                {
                                    failed = true;
                                    return;
                                }
                            }
                        });

                        if (failed)
                        {
                            return false;
                        }
                    }

                    Ptxop = ptxop;
                    return true;
                }
            });
        }

        public override int In
        {
            get
            {
                var sigs = Sig != null ? Sig.MkArray().ToReadOnly() : Ptxop.PtxopSigs();
                var in_counts = sigs.Select(sig => sig.Operands.Count()).ToReadOnly();
                return in_counts.Distinct().Single();
            }
        }

        public override int Out
        {
            get
            {
                var sigs = Sig != null ? Sig.MkArray().ToReadOnly() : Ptxop.PtxopSigs();
                var out_counts = sigs.Select(sig => sig.Destination == null ? 0 : 1).ToReadOnly();
                return out_counts.Distinct().Single();
            }
        }

        public override Type Type
        {
            get
            {
                var sigs = Sig != null ? Sig.MkArray().ToReadOnly() : Ptxop.PtxopSigs();
                var p_dest = sigs.Select(sig => sig.Destination).Distinct().Single();
                if (p_dest == null)
                {
                    return typeof(void);
                }
                else
                {
                    var op_dest = Ptxop.PtxopState().Operands[p_dest.Decl];
                    return op_dest.Type;
                }
            }
        }
    }
}