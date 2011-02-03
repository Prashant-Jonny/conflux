using System;
using Conflux.Core.Api;

namespace Conflux.Runtime.Cuda.Api
{
    internal abstract partial class Impl : ISyncApi
    {
        public void SyncThreads(params Object[] keys)
        {
            Ctm.SyncThreads(0);
        }
    }
}
