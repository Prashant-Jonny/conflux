namespace Conflux.Core.Configuration.Cuda
{
    public enum MethodStatus
    {
        IsRedirected,
        HasSpecialSemantics,
        CanBeExecutedOnDevice,
        MustNotBeExecutedOnDevice,
    }
}