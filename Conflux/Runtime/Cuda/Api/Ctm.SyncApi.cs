using System.Runtime.CompilerServices;
using Libptx.Bindings;

namespace Conflux.Runtime.Cuda.Api
{
    internal static partial class Ctm
    {
        [Ptx("bar.sync %d"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static void SyncThreads(int d);
    }
}
