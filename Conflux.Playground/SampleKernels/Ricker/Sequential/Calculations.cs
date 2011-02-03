using System;
using Conflux.Playground.SampleKernels.Matmul.Domain;
using Conflux.Playground.SampleKernels.Ricker.Domain;

namespace Conflux.Playground.SampleKernels.Ricker.Sequential
{
    public class Calculations
    {
        private readonly Matrix<Cell> _matrix;
        private const int matrixSize = 202;

        public Calculations()
        {
            _matrix = new Matrix<Cell>(matrixSize, matrixSize);
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = 0; j < matrixSize; j++)
                {
                    _matrix[i, j] = new Cell();
                }
            }
        }

        public void DoCalculate()
        {
            using (var dumper = new Dumper())
            {
                const float ρ = 1500.0f;
                const float Vp = 1000.0f;
                const float dx = 5.0f;
                const float dt = 0.001f;

                for (var t = 0.0f; t < 2.0f; t += dt)
//                for (var t = 0.0f; t < 0.5f; t += dt)
                {
//                     v=(π*f0*(t-t1))^2
//                     τ(100,100)=τ(100,100)+(1.-2.*v)*exp(-v)
                    const float t1 = 0.04f;
                    const float f0 = 40.0f;
                    var v = (float)Math.Pow((Math.PI * f0 * (t - t1)), 2);
                    const int center = (matrixSize - 2) / 2;
                    var c = _matrix[center, center];
                    c.Tau += (1.0f - 2.0f * v) * (float)Math.Exp(-v);

//                     D=dt/(ρ(i,j)*dx)/5000000
//                     Uх(i,j)=Uх(i,j)+(τ(i,j)-τ(i-1,j))*D
//                     Uz(i,j)=Uz(i,j)+(τ(i,j)-τ(i,j-1))*D
                    const float D = dt / (ρ * dx) / 5000000;
                    for (var row = 1; row < matrixSize - 1; row++)
                    {
                        for (var col = 1; col < matrixSize - 1; col++)
                        {
                            var cell = _matrix[row, col];
                            cell.Ux += (cell.Tau - _matrix[row - 1, col].Tau) * D;
                            cell.Uz += (cell.Tau - _matrix[row, col - 1].Tau) * D;
                        }
                    }

//                    Q=5000000*(dt*ρ(i,j)*Vp(i,j)*Vp(i,j))/dx
//                    τ(i,j)=τ(i,j)+(Uх(i+1,j)-Uх(i,j)+Uz(i,j+1)-Uz(i,j))*Q
                    const float Q = 5000000 * ((dt * ρ) * Vp * Vp) / dx;
                    for (var row = 1; row < matrixSize - 1; row++)
                    {
                        for (var col = 1; col < matrixSize - 1; col++)
                        {
                            var cell = _matrix[row, col];
                            cell.Tau += (_matrix[row + 1, col].Ux - cell.Ux + 
                                _matrix[row, col + 1].Uz - cell.Uz) * Q;
                        }
                    }

                    const float t0 = 0.04f;
                    const float dt0 = 0.03f;
                    var i = (int)((t - t0) / dt0);
                    if ((0 <= (t - t0 - dt0 * i)) && ((t - t0 - dt0 * i) < dt))
                    {
//                        dumper.Dump(_matrix);
                    }
                }
            }
        }
    }

}