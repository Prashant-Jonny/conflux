using System.Collections.ObjectModel;
using Libptx.Common.Enumerations;
using Libptx.Expressions.Addresses;
using Libptx.Expressions.Slots;
using Truesight.Decompiler.Hir;

namespace Conflux.Runtime.Cuda.Jit.Codegen.Layouts
{
    internal class ArrayLayout : Layout
    {
        public Node Node { get; private set; }
        public space Space { get; private set; }
        public Address Ptr { get; private set; }
        public ReadOnlyCollection<Reg> Dims { get; private set; }

        public ArrayLayout(Node node, space space, Address ptr, ReadOnlyCollection<Reg> dims)
        {
            Node = node;
            Space = space;
            Ptr = ptr;
            Dims = dims;
        }
    }
}