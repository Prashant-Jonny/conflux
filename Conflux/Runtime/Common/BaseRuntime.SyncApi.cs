using System;
using Conflux.Core.Api;

namespace Conflux.Runtime.Common
{
    internal abstract partial class BaseRuntime<TConfig, TJit> : ISyncApi
    {
        void ISyncApi.SyncThreads(params Object[] keys) { SyncThreads(keys); }
        protected virtual void SyncThreads(params Object[] keys)
        {
            throw new NotSupportedException("Calls to ISyncApi.SyncThreads should have been crosscompiled by the runtime.");
        }
    }
}