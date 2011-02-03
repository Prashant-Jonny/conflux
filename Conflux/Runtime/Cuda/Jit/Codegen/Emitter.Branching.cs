using System;
using Conflux.Runtime.Cuda.Jit.Codegen.Snippets;
using Libptx.Expressions.Addresses;
using Libptx.Instructions.ControlFlow;
using XenoGears.Strings;

namespace Conflux.Runtime.Cuda.Jit.Codegen
{
    internal partial class Emitter
    {
        public Label def_label() { return def_label("$" + Guid.NewGuid().ToString().Slice(0, 4)); }
        public Label def_label(String infix) { if (!infix.StartsWith("$")) infix += ("_" + Guid.NewGuid().ToString().Slice(0, 4)); return new Label { Name = infix }; }
        public Emitter def_label(out Label label) { label = def_label(); return this; }
        public Emitter def_label(String infix, out Label label) { label = def_label(infix); return this; }
        public Emitter label(Label label) { _ptx.Add(label); return this; }
        public Emitter bra(Label label) { _ptx.Add(new bra{tgt = label}); return this; }
        public Emitter bra(Snippet expr, Label label) { var pred = def_local(typeof(bool)); this.ld(expr).st(pred); _ptx.Add(new bra{Guard = pred, tgt = label}); return this; }
    }
}
