using Libcuda.DataTypes;

namespace Conflux.Runtime.Cpu.Jit
{
    internal interface IBlockRunner
    {
        void RunBlock(int3 blockIdx);
    }
}