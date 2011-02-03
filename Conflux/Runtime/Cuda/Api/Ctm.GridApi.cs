using System.Runtime.CompilerServices;
using Libcuda.DataTypes;
using Libptx.Bindings;

namespace Conflux.Runtime.Cuda.Api
{
    internal static partial class Ctm
    {
        [Ptx("%nctaid")]
        extern public static dim3 GridDim { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Ptx("%ntid")]
        extern public static dim3 BlockDim { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Ptx("%ctaid")]
        extern public static int3 BlockIdx { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Ptx("%tid")]
        extern public static int3 ThreadIdx { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}
