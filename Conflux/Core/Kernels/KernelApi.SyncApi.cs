using System;
using System.Runtime.CompilerServices;
using Conflux.Core.Api;

namespace Conflux.Core.Kernels
{
    public abstract partial class KernelApi : ISyncApi
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual void SyncThreads(params Object[] keys);
        void ISyncApi.SyncThreads(params Object[] keys) { SyncThreads(keys); }
    }
}