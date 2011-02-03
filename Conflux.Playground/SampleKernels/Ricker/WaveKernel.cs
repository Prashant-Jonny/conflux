using Conflux.Core.Configuration;
using Libcuda.DataTypes;
using Conflux.Core.Kernels;
using Conflux.Core.Annotations.Semantics;
using Conflux.Playground.SampleKernels.Matmul.Domain;
using Conflux.Playground.SampleKernels.Ricker.Domain;
using XenoGears.Assertions;

namespace Conflux.Playground.SampleKernels.Ricker
{
    public abstract class WaveKernel : Kernel<Matrix<Cell>, Matrix<Cell>>
    {
        [Result] protected Matrix<Cell> _matrix;

        protected override void Initialize(IGridConfig gridCfg, Matrix<Cell> input)
        {
            _matrix = input;
            (_matrix.Height == _matrix.Width).AssertTrue();

            (gridCfg.GridDim == null && gridCfg.BlockDim == null).AssertTrue();
            gridCfg.SetDims(new dim3(1), new dim3(64));
        }

        protected override void RunKernel()
        {
            var row = ThreadIdx.X / 16;
            var col = ThreadIdx.X % 16;
            if (_matrix.Height <= row || _matrix.Width <= col) return;

            const float ρ = 1500.0f;
            const float Vp = 1000.0f;
            const float dx = 5.0f;
            const float dt = 0.001f;

            for (var t = 0.0f; t < 2.0f; t += 0.001f)
            {
                // step 0: recalculate τ[100, 100]
                if (row == 0 && col == 0)
                {
//                    v=(π*f0*(t-t1))^2
                    const float t1 = 0.04f;
                    const float f0 = 40.0f;
                    var v = Powf((PIf * f0 * (t - t1)), 2);

//                    τ(100,100)=τ(100,100)+(1.-2.*v)*exp(-v)
                    var center = (_matrix.Width - 2)/2;
                    var cell = _matrix[center, center];
                    cell.Tau += (1.0f - 2.0f * v) * Expf(-v);
                    _matrix[center, center] = cell;
                }
                SyncThreads();

                // step 1: recalculate ux and uz
//                D=dt/(ρ(i,j)*dx)/5000000
//                Uх(i,j)=Uх(i,j)+(τ(i,j)-τ(i-1,j))*D
//                Uz(i,j)=Uz(i,j)+(τ(i,j)-τ(i,j-1))*D
                const float D = dt / (ρ * dx) / 5000000;
                for (var i = row + 1; i < _matrix.Height; i += 16)
                {
                    for (var j = col + 1; j < _matrix.Width; j += 16)
                    {
                        var cell = _matrix[row, col];
                        cell.Ux += (cell.Tau - _matrix[row - 1, col].Tau) * D;
                        cell.Uz += (cell.Tau - _matrix[row, col - 1].Tau) * D;
                        _matrix[row, col] = cell;
                    }
                }
                SyncThreads();

                // step 2: recalculate τ
//                Q=5000000*(dt*ρ(i,j)*Vp(i,j)*Vp(i,j))/dx
//                τ(i,j)=τ(i,j)+(Uх(i+1,j)-Uх(i,j)+Uz(i,j+1)-Uz(i,j))*Q
                const float Q = 5000000 * ((dt * ρ) * Vp * Vp) / dx;
                for (var i = row + 1; i < _matrix.Height; i += 16)
                {
                    for (var j = col + 1; j < _matrix.Width; j += 16)
                    {
                        var cell = _matrix[row, col];
                        cell.Tau += (_matrix[row + 1, col].Ux - cell.Ux + _matrix[row, col + 1].Uz - cell.Uz) * Q;
                        _matrix[row, col] = cell;
                    }
                }
                SyncThreads();
            }
        }
    }
}
