using System;
using Conflux.Core.Configuration;

namespace Conflux.Core.Kernels
{
    public interface IKernel
    {
        // State 1. Initialization (single-threaded, runs on host)
        //
        // used to assign parameters to internal properties
        // might also contain some validation
        // also can host custom partitioning algorithms
        //
        // an example of optimization this architecture brings to us:
        // initializer might detect that C is small enough to be stored in shared memory
        // then it abandons all dims it received from outside, and runs a single block
        // that shares the resulting array => profit!

        void Initialize(IGridConfig gridCfg, params Object[] args);

        // State 2. Kernel execution (multi-threaded, runs on device)
        //
        // specifies logic of a single thread.
        // blid and tid ain't passed as parameters since they belong to IGridApi
        // also. implements initialization logic performed once before the calculations
        // e.g. for matmul it's allocation of the result matrix

        void RunKernel();

        // State 3. Getting the kernel result (single-threaded, runs on host)
        //
        // gets the result returned by execute
        // default implementation returns a single prop annotated with [Result]
        // this needs to be overriden if the kernel returns multiple result values
        // the values should be wrapped and returned in a single instance
        //
        // Conflux JIT will insert device -> host transfers for the properties

        Object FetchResult();
    }
}