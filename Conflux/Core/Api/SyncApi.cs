using System;
using System.Diagnostics;
using Conflux.Core.Api.Registry;
using Conflux.Runtime.Common.Registry;

namespace Conflux.Core.Api
{
    [Api, DebuggerNonUserCode]
    public static class SyncApi
    {
        public static void SyncThreads(params Object[] keys)
        {
            Runtimes.ActiveApi.SyncThreads(keys);
        }
    }
}