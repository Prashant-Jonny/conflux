using System;
using Conflux.Core.Api;
using Libptx;

namespace Conflux.Runtime.Cuda.Api
{
    // todo. below you can see a quick & dirty implementation
    // 1) most likely, rounding and precision won't conform to IEEE standards
    // e.g. expm1/log1p won't work as it should for very small values of x
    // 2) i'm pretty sure that implementations below have poor performance
    // 3) finally, I don't even guarantee that code below works 100% correctly

    internal abstract partial class Impl : IMathApi
    {
        protected const float Ef = MathApi.Ef;
        protected const double E = MathApi.E;
        protected const float PIf = MathApi.PIf;
        protected const double PI = MathApi.PI;
        protected const float PosInff = MathApi.PosInff;
        protected const double PosInf = MathApi.PosInf;
        protected const float NegInff = MathApi.NegInff;
        protected const double NegInf = MathApi.NegInf;

        public float Fmaf(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public double Fma(double x, double y, double z)
        {
            return Ctm.Fma(x, y, z);
        }

        public float Madf(float x, float y, float z)
        {
            return Ctm.Madf(x, y, z);
        }

        public double Mad(double x, double y, double z)
        {
            return Ctm.Mad(x, y, z);
        }

        public int Abs(int x)
        {
            return Ctm.Abs(x);
        }

        public long Labs(long x)
        {
            return Ctm.Labs(x);
        }

        public float Fabsf(float x)
        {
            return Ctm.Fabsf(x);
        }

        public double Fabs(double x)
        {
            return Ctm.Fabs(x);
        }

        public int Min(int x, int y)
        {
            return Ctm.Min(x, y);
        }

        public uint Umin(uint x, uint y)
        {
            return Ctm.Umin(x, y);
        }

        public long Lmin(long x, long y)
        {
            return Ctm.Lmin(x, y);
        }

        public ulong Ulmin(ulong x, ulong y)
        {
            return Ctm.Ulmin(x, y);
        }

        public float Fminf(float x, float y)
        {
            return Ctm.Fminf(x, y);
        }

        public double Fmin(double x, double y)
        {
            return Ctm.Fmin(x, y);
        }

        public int Max(int x, int y)
        {
            return Ctm.Max(x, y);
        }

        public uint Umax(uint x, uint y)
        {
            return Ctm.Umax(x, y);
        }

        public long Lmax(long x, long y)
        {
            return Ctm.Lmax(x, y);
        }

        public ulong Ulmax(ulong x, ulong y)
        {
            return Ctm.Ulmax(x, y);
        }

        public float Fmaxf(float x, float y)
        {
            return Ctm.Fmaxf(x, y);
        }

        public double Fmax(double x, double y)
        {
            return Ctm.Fmax(x, y);
        }

        public float Sqrtf(float x)
        {
            return Ctm.Sqrtf(x);
        }

        public double Sqrt(double x)
        {
            return Ctm.Sqrt(x);
        }

        public float Rsqrtf(float x)
        {
            return Ctm.Rsqrtf(x);
        }

        public double Rsqrt(double x)
        {
            return Ctm.Rsqrt(x);
        }

        public float Cbrtf(float x)
        {
            return Powf(x, 1f / 3);
        }

        public double Cbrt(double x)
        {
            return Pow(x, 1d / 3);
        }

        public float Rcbrtf(float x)
        {
            return Powf(x, -1f / 3);
        }

        public double Rcbrt(double x)
        {
            return Pow(x, -1d / 3);
        }

        public float Powf(float x, float y)
        {
            return Exp2f(Log2f(x) * y);
        }

        public double Pow(double x, double y)
        {
            throw new NotImplementedException();
        }

        public float Exp2f(float x)
        {
            return Ctm.Exp2f(x);
        }

        public double Exp2(double x)
        {
            throw new NotImplementedException();
        }

        public float Exp10f(float x)
        {
            return Powf(10, x);
        }

        public double Exp10(double x)
        {
            return Pow(10, x);
        }

        public float Expm1f(float x)
        {
            return Expf(x) - 1;
        }

        public double Expm1(double x)
        {
            return Exp(x) - 1;
        }

        public float Expf(float x)
        {
            return Powf(Ef, x);
        }

        public double Exp(double x)
        {
            return Pow(E, x);
        }

        public float Logf(float x)
        {
            return Log2f(x) / Log2f(Ef);
        }

        public double Log(double x)
        {
            return Log2(x) / Log2(E);
        }

        public float Log2f(float x)
        {
            return Ctm.Log2f(x);
        }

        public double Log2(double x)
        {
            throw new NotImplementedException();
        }

        public float Log10f(float x)
        {
            return Log2f(x) / Log2f(10);
        }

        public double Log10(double x)
        {
            return Log2(x) / Log2(10);
        }

        public float Log1pf(float x)
        {
            return Logf(x + 1);
        }

        public double Log1p(double x)
        {
            return Log(x + 1);
        }

        public float Logf(float x, float @base)
        {
            return Log2f(x) / Log2f(@base);
        }

        public double Log(double x, double @base)
        {
            return Log2(x) / Log2(@base);
        }

        public float Sinf(float x)
        {
            return Ctm.Sinf(x);
        }

        public double Sin(double x)
        {
            throw new NotImplementedException();
        }

        public float Sinpif(float x)
        {
            return Sinf(PIf * x);
        }

        public double Sinpi(double x)
        {
            return Sin(PI * x);
        }

        public float Cosf(float x)
        {
            return Ctm.Cosf(x);
        }

        public double Cos(double x)
        {
            throw new NotImplementedException();
        }

        public void Sincosf(float x, out float sin, out float cos)
        {
            sin = Sinf(x);
            cos = Cosf(x);
        }

        public void Sincos(double x, out double sin, out double cos)
        {
            sin = Sin(x);
            cos = Cos(x);
        }

        public float Tanf(float x)
        {
            return Sinf(x) / Cosf(x);
        }

        public double Tan(double x)
        {
            return Sin(x) / Cos(x);
        }

        public float Asinf(float x)
        {
            throw new NotImplementedException();
        }

        public double Asin(double x)
        {
            throw new NotImplementedException();
        }

        public float Acosf(float x)
        {
            throw new NotImplementedException();
        }

        public double Acos(double x)
        {
            throw new NotImplementedException();
        }

        public float Atanf(float x)
        {
            throw new NotImplementedException();
        }

        public double Atan(double x)
        {
            throw new NotImplementedException();
        }

        public float Atan2f(float y, float x)
        {
            throw new NotImplementedException();
        }

        public double Atan2(double y, double x)
        {
            throw new NotImplementedException();
        }

        public float Sinhf(float x)
        {
            throw new NotImplementedException();
        }

        public double Sinh(double x)
        {
            throw new NotImplementedException();
        }

        public float Coshf(float x)
        {
            throw new NotImplementedException();
        }

        public double Cosh(double x)
        {
            throw new NotImplementedException();
        }

        public float Tanhf(float x)
        {
            throw new NotImplementedException();
        }

        public double Tanh(double x)
        {
            throw new NotImplementedException();
        }

        public float Asinhf(float x)
        {
            throw new NotImplementedException();
        }

        public double Asinh(double x)
        {
            throw new NotImplementedException();
        }

        public float Acoshf(float x)
        {
            throw new NotImplementedException();
        }

        public double Acosh(double x)
        {
            throw new NotImplementedException();
        }

        public float Atanhf(float x)
        {
            throw new NotImplementedException();
        }

        public double Atanh(double x)
        {
            throw new NotImplementedException();
        }

        public float Floorf(float x)
        {
            return Ctm.Floorf(x);
        }

        public double Floor(double x)
        {
            return Ctm.Floor(x);
        }

        public float Ceilf(float x)
        {
            return Ctm.Ceilf(x);
        }

        public double Ceil(double x)
        {
            return Ctm.Ceil(x);
        }

        public float Truncf(float x)
        {
            return Ctm.Truncf(x);
        }

        public double Trunc(double x)
        {
            return Ctm.Trunc(x);
        }

        public float Roundf(float x)
        {
            throw new NotImplementedException();
        }

        public double Round(double x)
        {
            throw new NotImplementedException();
        }

        public bool IsInff(float x)
        {
            return Fabsf(x) == PosInff;
        }

        public bool IsInf(double x)
        {
            return Fabs(x) == PosInf;
        }

        public bool IsNanf(float x)
        {
            return Fabsf(x) > PosInff;
        }

        public bool IsNan(double x)
        {
            return Fabs(x) > PosInf;
        }

        public bool IsFinitef(float x)
        {
            return Fabsf(x) < PosInff;
        }

        public bool IsFinite(double x)
        {
            return Fabs(x) < PosInf;
        }
    }
}
