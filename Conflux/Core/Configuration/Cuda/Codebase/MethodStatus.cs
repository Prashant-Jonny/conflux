namespace Conflux.Core.Configuration.Cuda.Codebase
{
    public enum MethodStatus
    {
        IsRedirected,
        HasSpecialSemantics,
        CanBeExecutedOnDevice,
        MustNotBeExecutedOnDevice,
    }
}