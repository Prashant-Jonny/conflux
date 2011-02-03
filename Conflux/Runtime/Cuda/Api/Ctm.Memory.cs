using System;
using System.Runtime.CompilerServices;

namespace Conflux.Runtime.Cuda.Api
{
    internal static partial class Ctm
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object Malloc(Type type);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object Malloc(Type type, int width);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object Malloc(Type type, int height, int width);
    }
}
