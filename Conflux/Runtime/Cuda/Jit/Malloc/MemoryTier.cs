namespace Conflux.Runtime.Cuda.Jit.Malloc
{
    internal enum MemoryTier
    {
        Private = 1,
        Shared = 2,
        Global = 3,
    }
}