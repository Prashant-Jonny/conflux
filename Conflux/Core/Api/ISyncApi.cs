using System;
using Conflux.Core.Api.Registry;

namespace Conflux.Core.Api
{
    [Api]
    public interface ISyncApi
    {
        void SyncThreads(params Object[] keys);
    }
}
