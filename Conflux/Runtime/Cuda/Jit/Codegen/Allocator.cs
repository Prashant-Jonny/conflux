using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conflux.Runtime.Cuda.Jit.Codegen.Layouts;
using Conflux.Runtime.Cuda.Jit.Malloc;
using Libcuda.DataTypes;
using Libptx.Common.Enumerations;
using Libptx.Expressions.Slots;
using Libptx.Functions;
using Libptx.Instructions.MovementAndConversion;
using Libptx.Statements;
using Truesight.Decompiler.Hir.Core.Expressions;
using Truesight.Decompiler.Hir.Core.Symbols;
using Truesight.Decompiler.Hir.TypeInference;
using XenoGears.Assertions;
using XenoGears.Collections.Dictionaries;
using XenoGears.Functional;
using XenoGears.Reflection;
using PtxModule = Libptx.Module;
using PtxEntry = Libptx.Functions.Entry;
using PtxParams = Libptx.Functions.Params;
using PtxExpression = Libptx.Expressions.Expression;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal class Allocator
    {
        private MethodInfo _kernel { get { return JitContext.Current.Kernel; } }
        private PtxModule _module { get { return JitContext.Current.Module; } }
        private PtxEntry _entry { get { return JitContext.Current.Entry; } }
        private PtxParams _params { get { return JitContext.Current.Params; } }
        private IList<Statement> _ptx { get { return _entry.Stmts; } }
        private AllocationScheme _scheme { get { return JitContext.Current.AllocationScheme; } }

        public OrderedDictionary<Local, Layout> Locals { get { return _locals_cache; } }
        private OrderedDictionary<Local, Layout> _locals_cache = new OrderedDictionary<Local, Layout>();
        public Layout this[Ref @ref]
        {
            get
            {
                @ref.AssertNotNull();
                var local = @ref.Sym.AssertCast<Local>();

                return _locals_cache.GetOrCreate(local, () =>
                {
                    var t = @ref.Type();
                    if (_scheme[@ref] == MemoryTier.Private)
                    {
                        if (t.IsArray)
                        {
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            (t.IsCudaPrimitive() || t.IsCudaVector()).AssertTrue();
                            var slot = new Reg{Type = @ref.Type(), Name = @ref.Sym.Name};
                            return new SlotLayout(@ref, slot);
                        }
                    }
                    else if (_scheme[@ref] == MemoryTier.Shared)
                    {
                        if (t.IsArray)
                        {
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
                    }
                    else if (_scheme[@ref] == MemoryTier.Global)
                    {
                        if (t.IsArray)
                        {
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }
                });
            }
        }

        public OrderedDictionary<FieldInfo, Layout> Fields { get { return _fi_cache; } }
        private OrderedDictionary<FieldInfo, Layout> _fi_cache = new OrderedDictionary<FieldInfo, Layout>();
        public Layout this[Fld fld]
        {
            get
            {
                fld.AssertNotNull();

                var fi = fld.Field;
                fi.AssertNotNull();
                _kernel.DeclaringType.Hierarchy().Contains(fi.DeclaringType).AssertTrue();

                return _fi_cache.GetOrCreate(fi, () =>
                {
                    var t = fi.FieldType;
                    if (_scheme[fi] == MemoryTier.Private)
                    {
                        if (t.IsArray)
                        {
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
                    }
                    else if (_scheme[fi] == MemoryTier.Shared)
                    {
                        if (t.IsArray)
                        {
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
                    }
                    else if (_scheme[fi] == MemoryTier.Global)
                    {
                        if (fi.FieldType.IsArray)
                        {
                            var p_raw = new Var{Space = space.param, Name = "parm_" + fi.Name, Type = typeof(uint)};
                            var raw = new Reg{Type = typeof(uint), Name = fi.Name};
                            _ptx.Add(new ld{ss = space.param, type = typeof(uint), d = raw, a = p_raw});

                            var p_height = new Var{Space = space.param, Name = "parm_" + fi.Name + "_height", Type = typeof(int)};
                            var height = new Reg{Type = typeof(int), Name = fi.Name + "_height"};
                            _ptx.Add(new ld{ss = space.param, type = typeof(int), d = height, a = p_height});

                            var p_width = new Var{Space = space.param, Name = "parm_" + fi.Name + "_width", Type = typeof(int)};
                            var width = new Reg{Type = typeof(int), Name = fi.Name + "_width"};
                            _ptx.Add(new ld{ss = space.param, type = typeof(int), d = width, a = p_width});

                            _params.AddElements(p_raw, p_height, p_width);
                            return new ArrayLayout(fld, space.global, raw, new []{height, width}.ToReadOnly());
                        }
                        else
                        {
                            (t.IsCudaPrimitive() || t.IsCudaVector()).AssertTrue();

                            var p_value = new Var{Space = space.param, Name = "parm_" + fi.Name, Type = t};
                            var value = new Reg{Type = t, Name = fi.Name};
                            _ptx.Add(new ld{ss = space.param, type = t, d = value, a = p_value});

                            _params.Add(p_value);
                            return new SlotLayout(fld, value);
                        }
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }
                });
            }
        }
    }
}