using Conflux.Core.Api;

namespace Conflux.Runtime
{
    internal interface IRuntimeApi : IGridApi, ISyncApi, IMathApi
    {
    }
}