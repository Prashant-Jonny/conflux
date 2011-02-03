using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Conflux.Core.Annotations
{
    [DebuggerNonUserCode]
    public static class Hints
    {
        public static AllocHint Alloc { get { return new AllocHint(); } }
        public static SemanticsHint Semantics { get { return new SemanticsHint(); } }
        public static SharingHint Sharing { get { return new SharingHint(); } }

        [DebuggerNonUserCode]
        public class AllocHint
        {
            internal AllocHint() { }

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public AllocHint Host(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public AllocHint Spilled(params Object[] vars);

//            public AllocHint Alloc { get { return new AllocHint(); } }
            public SemanticsHint Semantics { get { return new SemanticsHint(); } }
            public SharingHint Sharing { get { return new SharingHint(); } }
        }

        [DebuggerNonUserCode]
        public class SemanticsHint
        {
            internal SemanticsHint() { }

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SemanticsHint Constant(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SemanticsHint Shape1D(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SemanticsHint Shape2D(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SemanticsHint Shape3D(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SemanticsHint Result(params Object[] vars);

            public AllocHint Alloc { get { return new AllocHint(); } }
//            public SemanticHint Semantics { get { return new SemanticHint(); } }
            public SharingHint Sharing { get { return new SharingHint(); } }
        }

        [DebuggerNonUserCode]
        public class SharingHint
        {
            internal SharingHint() { }

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SharingHint Private(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SharingHint Local(params Object[] vars);

            [MethodImpl(MethodImplOptions.InternalCall)]
            extern public SharingHint Global(params Object[] vars);

            public AllocHint Alloc { get { return new AllocHint(); } }
            public SemanticsHint Semantics { get { return new SemanticsHint(); } }
//            public SharingHint Sharing { get { return new SharingHint(); } }
        }
    }
}