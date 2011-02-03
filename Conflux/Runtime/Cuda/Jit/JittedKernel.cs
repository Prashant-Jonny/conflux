using System;
using System.Linq;
using Conflux.Core.Configuration.Cuda;
using Conflux.Runtime.Cuda.Jit.Codegen.Layouts;
using Libcuda.DataTypes;
using Libcuda;
using Libcuda.Api.Run;
using Truesight.Decompiler.Hir.Core.ControlFlow;
using XenoGears;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection.Attributes;

namespace Conflux.Runtime.Cuda.Jit
{
    internal class JittedKernel
    {
        private readonly CudaConfig _cfg;
        public CudaConfig Cfg { get { return _cfg; } }

        private readonly Block _hir;
        internal Block Hir { get { return _hir; } }

        private readonly String _ptx;
        public String Ptx { get { return _ptx; } }

        private readonly Func<Object, KernelArguments> _memcpyHostToDevice;
        private readonly Action<KernelResult, Object> _memcpyDeviceToHost;

        public JittedKernel(JitContext ctx)
        {
            _cfg = ctx.Cfg.Clone();
            _ptx = ctx.Ptx;
            _hir = ctx.Hir.Fluent(blk => blk.FreezeForever());

            var flds = ctx.Allocator.Fields;
            _memcpyHostToDevice = kernel => 
            {
                var args = new KernelArguments();
                flds.Keys.ForEach(fld =>
                {
                    var value = fld.GetValue(kernel);
                    if (flds[fld] is SlotLayout)
                    {
                        args.Add(value.In());
                    }
                    else if (flds[fld] is ArrayLayout)
                    {
                        var arr = value.AssertCast<Array>();
                        args.Add(arr.InOut());

                        var rank = arr.GetType().GetArrayRank();
                        0.UpTo(rank - 1).ForEach(i => args.Add(arr.GetLength(i).In()));
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }
                });

                return args;
            };

            _memcpyDeviceToHost = (result, kernel) =>
            {
                var idx = 0;
                flds.Keys.ForEach(fld =>
                {
                    Object value;
                    if (flds[fld] is SlotLayout)
                    {
                        value = result[idx];
                        idx += 1;
                    }
                    else if (flds[fld] is ArrayLayout)
                    {
                        value = result[idx];
                        idx += 3;
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }

                    fld.SetValue(kernel, value);
                });
            };
        }

        public void Run(dim3 gridDim, dim3 blockDim, Object kernel_instance)
        {
            // todo. due to a bug in Libcuda/nvcuda we cannot specify compilation Target
            // when we use default target (TargetFromContext) everything works fine tho
            // so let's stick to this solution for now
            (Cfg.Target == CudaVersions.HardwareIsa).AssertTrue();
            using (var jitted_ptx = Ptx.JitKernel(blockDim))
//            using (var jitted_ptx = Ptx.JitKernel(blockDim, Cfg.Target))
            {
                using (var kernel_args = _memcpyHostToDevice(kernel_instance))
                {
                    var kernel_result = jitted_ptx.Run(gridDim, blockDim, kernel_args);
                    _memcpyDeviceToHost(kernel_result, kernel_instance);
                }
            }
        }
    }
}