using Libptx.Expressions.Slots;
using Truesight.Decompiler.Hir;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Layouts
{
    internal class SlotLayout : Layout
    {
        public Node Node { get; private set; }
        public Slot Slot { get; private set; }

        public SlotLayout(Node node, Slot slot)
        {
            Node = node;
            Slot = slot;
        }
    }
}
