using System;
using Conflux.Core.Api;

namespace Conflux.Runtime.Common
{
    // todo. below you can see a quick & dirty implementation
    // 1) most likely, rounding and precision won't conform to IEEE standards
    // e.g. expm1/log1p won't work as it should for very small values of x
    // 2) i'm pretty sure that implementations below have poor performance
    // 3) finally, I don't even guarantee that code below works 100% correctly

    internal abstract partial class BaseRuntime<TConfig, TJit> : IMathApi
    {
        protected const float Ef = MathApi.Ef;
        protected const double E = MathApi.E;
        protected const float PIf = MathApi.PIf;
        protected const double PI = MathApi.PI;
        protected const float PosInff = MathApi.PosInff;
        protected const double PosInf = MathApi.PosInf;
        protected const float NegInff = MathApi.NegInff;
        protected const double NegInf = MathApi.NegInf;

        float IMathApi.Fmaf(float x, float y, float z) { return Fmaf(x, y, z); }
        protected virtual float Fmaf(float x, float y, float z)
        {
            return x * y + z;
        }

        double IMathApi.Fma(double x, double y, double z) { return Fma(x, y, z); }
        protected virtual double Fma(double x, double y, double z)
        {
            return x * y + z;
        }

        float IMathApi.Madf(float x, float y, float z) { return Madf(x, y, z); }
        protected virtual float Madf(float x, float y, float z)
        {
            return x * y + z;
        }

        double IMathApi.Mad(double x, double y, double z) { return Mad(x, y, z); }
        protected virtual double Mad(double x, double y, double z)
        {
            return x * y + z;
        }

        int IMathApi.Abs(int x) { return Abs(x); }
        protected virtual int Abs(int x)
        {
            return Math.Abs(x);
        }

        long IMathApi.Labs(long x) { return Labs(x); }
        protected virtual long Labs(long x)
        {
            return Math.Abs(x);
        }

        float IMathApi.Fabsf(float x) { return Fabsf(x); }
        protected virtual float Fabsf(float x)
        {
            return Math.Abs(x);
        }

        double IMathApi.Fabs(double x) { return Fabs(x); }
        protected virtual double Fabs(double x)
        {
            return Math.Abs(x);
        }

        int IMathApi.Min(int x, int y) { return Min(x, y); }
        protected virtual int Min(int x, int y)
        {
            return Math.Min(x, y);
        }

        uint IMathApi.Umin(uint x, uint y) { return Umin(x, y); }
        protected virtual uint Umin(uint x, uint y)
        {
            return Math.Min(x, y);
        }

        long IMathApi.Lmin(long x, long y) { return Lmin(x, y); }
        protected virtual long Lmin(long x, long y)
        {
            return Math.Min(x, y);
        }

        ulong IMathApi.Ulmin(ulong x, ulong y) { return Ulmin(x, y); }
        protected virtual ulong Ulmin(ulong x, ulong y)
        {
            return Math.Min(x, y);
        }

        float IMathApi.Fminf(float x, float y) { return Fminf(x, y); }
        protected virtual float Fminf(float x, float y)
        {
            return Math.Min(x, y);
        }

        double IMathApi.Fmin(double x, double y) { return Fmin(x, y); }
        protected virtual double Fmin(double x, double y)
        {
            return Math.Min(x, y);
        }

        int IMathApi.Max(int x, int y) { return Max(x, y); }
        protected virtual int Max(int x, int y)
        {
            return Math.Max(x, y);
        }

        uint IMathApi.Umax(uint x, uint y) { return Umax(x, y); }
        protected virtual uint Umax(uint x, uint y)
        {
            return Math.Max(x, y);
        }

        long IMathApi.Lmax(long x, long y) { return Lmax(x, y); }
        protected virtual long Lmax(long x, long y)
        {
            return Math.Max(x, y);
        }

        ulong IMathApi.Ulmax(ulong x, ulong y) { return Ulmax(x, y); }
        protected virtual ulong Ulmax(ulong x, ulong y)
        {
            return Math.Max(x, y);
        }

        float IMathApi.Fmaxf(float x, float y) { return Fmaxf(x, y); }
        protected virtual float Fmaxf(float x, float y)
        {
            return Math.Max(x, y);
        }

        double IMathApi.Fmax(double x, double y) { return Fmax(x, y); }
        protected virtual double Fmax(double x, double y)
        {
            return Math.Max(x, y);
        }

        float IMathApi.Sqrtf(float x) { return Sqrtf(x); }
        protected virtual float Sqrtf(float x)
        {
            return (float)Math.Pow(x, 1f / 2);
        }

        double IMathApi.Sqrt(double x) { return Sqrt(x); }
        protected virtual double Sqrt(double x)
        {
            return Math.Pow(x, 1d / 2);
        }

        float IMathApi.Rsqrtf(float x) { return Rsqrtf(x); }
        protected virtual float Rsqrtf(float x)
        {
            return (float)Math.Pow(x, -1f / 2);
        }

        double IMathApi.Rsqrt(double x) { return Rsqrt(x); }
        protected virtual double Rsqrt(double x)
        {
            return Math.Pow(x, -1d / 2);
        }

        float IMathApi.Cbrtf(float x) { return Cbrtf(x); }
        protected virtual float Cbrtf(float x)
        {
            return (float)Math.Pow(x, 1f / 3);
        }

        double IMathApi.Cbrt(double x) { return Cbrt(x); }
        protected virtual double Cbrt(double x)
        {
            return Math.Pow(x, 1d / 3);
        }

        float IMathApi.Rcbrtf(float x) { return Rcbrtf(x); }
        protected virtual float Rcbrtf(float x)
        {
            return (float)Math.Pow(x, -1f / 3);
        }

        double IMathApi.Rcbrt(double x) { return Rcbrt(x); }
        protected virtual double Rcbrt(double x)
        {
            return Math.Pow(x, -1d / 3);
        }

        float IMathApi.Powf(float x, float y) { return Powf(x, y); }
        protected virtual float Powf(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        double IMathApi.Pow(double x, double y) { return Pow(x, y); }
        protected virtual double Pow(double x, double y)
        {
            return Math.Pow(x, y);
        }

        float IMathApi.Exp2f(float x) { return Exp2f(x); }
        protected virtual float Exp2f(float x)
        {
            return (float)Math.Pow(2f, x);
        }

        double IMathApi.Exp2(double x) { return Exp2(x); }
        protected virtual double Exp2(double x)
        {
            return Math.Pow(2d, x);
        }

        float IMathApi.Exp10f(float x) { return Exp10f(x); }
        protected virtual float Exp10f(float x)
        {
            return (float)Math.Pow(10f, x);
        }

        double IMathApi.Exp10(double x) { return Exp10(x); }
        protected virtual double Exp10(double x)
        {
            return Math.Pow(10d, x);
        }

        float IMathApi.Expm1f(float x) { return Expm1f(x); }
        protected virtual float Expm1f(float x)
        {
            return Expf(x) - 1f;
        }

        double IMathApi.Expm1(double x) { return Expm1(x); }
        protected virtual double Expm1(double x)
        {
            return Exp(x) - 1d;
        }

        float IMathApi.Expf(float x) { return Expf(x); }
        protected virtual float Expf(float x)
        {
            return (float)Math.Exp(x);
        }

        double IMathApi.Exp(double x) { return Exp(x); }
        protected virtual double Exp(double x)
        {
            return Math.Exp(x);
        }

        float IMathApi.Logf(float x) { return Logf(x); }
        protected virtual float Logf(float x)
        {
            return (float)Math.Log(x);
        }

        double IMathApi.Log(double x) { return Log(x); }
        protected virtual double Log(double x)
        {
            return Math.Log(x);
        }

        float IMathApi.Log2f(float x) { return Log2f(x); }
        protected virtual float Log2f(float x)
        {
            return (float)Math.Log(x, 2);
        }

        double IMathApi.Log2(double x) { return Log2(x); }
        protected virtual double Log2(double x)
        {
            return Math.Log(x, 2);
        }

        float IMathApi.Log10f(float x) { return Log10f(x); }
        protected virtual float Log10f(float x)
        {
            return (float)Math.Log10(x);
        }

        double IMathApi.Log10(double x) { return Log10(x); }
        protected virtual double Log10(double x)
        {
            return Math.Log10(x);
        }

        float IMathApi.Log1pf(float x) { return Log1pf(x); }
        protected virtual float Log1pf(float x)
        {
            return Logf(1 + x);
        }

        double IMathApi.Log1p(double x) { return Log1p(x); }
        protected virtual double Log1p(double x)
        {
            return Log(1 + x);
        }

        float IMathApi.Logf(float x, float @base) { return Logf(x, @base); }
        protected virtual float Logf(float x, float @base)
        {
            return (float)Math.Log(x, @base);
        }

        double IMathApi.Log(double x, double @base) { return Log(x, @base); }
        protected virtual double Log(double x, double @base)
        {
            return Math.Log(x, @base);
        }

        float IMathApi.Sinf(float x) { return Sinf(x); }
        protected virtual float Sinf(float x)
        {
            return (float)Math.Sin(x);
        }

        double IMathApi.Sin(double x) { return Sin(x); }
        protected virtual double Sin(double x)
        {
            return Math.Sin(x);
        }

        float IMathApi.Sinpif(float x) { return Sinpif(x); }
        protected virtual float Sinpif(float x)
        {
            return Sinf(x * PIf);
        }

        double IMathApi.Sinpi(double x) { return Sinpi(x); }
        protected virtual double Sinpi(double x)
        {
            return Sin(x * PI);
        }

        float IMathApi.Cosf(float x) { return Cosf(x); }
        protected virtual float Cosf(float x)
        {
            return (float)Math.Cos(x);
        }

        double IMathApi.Cos(double x) { return Cos(x); }
        protected virtual double Cos(double x)
        {
            return Math.Cos(x);
        }

        void IMathApi.Sincosf(float x, out float sin, out float cos) { Sincosf(x, out sin, out cos); }
        protected virtual void Sincosf(float x, out float sin, out float cos)
        {
            sin = Sinf(x);
            cos = Cosf(x);
        }

        void IMathApi.Sincos(double x, out double sin, out double cos) { Sincos(x, out sin, out cos); }
        protected virtual void Sincos(double x, out double sin, out double cos)
        {
            sin = Sin(x);
            cos = Cos(x);
        }

        float IMathApi.Tanf(float x) { return Tanf(x); }
        protected virtual float Tanf(float x)
        {
            return (float)Math.Tan(x);
        }

        double IMathApi.Tan(double x) { return Tan(x); }
        protected virtual double Tan(double x)
        {
            return Math.Tan(x);
        }

        float IMathApi.Asinf(float x) { return Asinf(x); }
        protected virtual float Asinf(float x)
        {
            return (float)Math.Asin(x);
        }

        double IMathApi.Asin(double x) { return Asin(x); }
        protected virtual double Asin(double x)
        {
            return Math.Asin(x);
        }

        float IMathApi.Acosf(float x) { return Acosf(x); }
        protected virtual float Acosf(float x)
        {
            return (float)Math.Acos(x);
        }

        double IMathApi.Acos(double x) { return Acos(x); }
        protected virtual double Acos(double x)
        {
            return Math.Acos(x);
        }

        float IMathApi.Atanf(float x) { return Atanf(x); }
        protected virtual float Atanf(float x)
        {
            return (float)Math.Atan(x);
        }

        double IMathApi.Atan(double x) { return Atan(x); }
        protected virtual double Atan(double x)
        {
            return Math.Atan(x);
        }

        float IMathApi.Atan2f(float y, float x) { return Atan2f(y, x); }
        protected virtual float Atan2f(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        double IMathApi.Atan2(double y, double x) { return Atan2(y, x); }
        protected virtual double Atan2(double y, double x)
        {
            return Math.Atan2(y, x);
        }

        float IMathApi.Sinhf(float x) { return Sinhf(x); }
        protected virtual float Sinhf(float x)
        {
            return (float)Math.Sinh(x);
        }

        double IMathApi.Sinh(double x) { return Sinh(x); }
        protected virtual double Sinh(double x)
        {
            return Math.Sinh(x);
        }

        float IMathApi.Coshf(float x) { return Coshf(x); }
        protected virtual float Coshf(float x)
        {
            return (float)Math.Cosh(x);
        }

        double IMathApi.Cosh(double x) { return Cosh(x); }
        protected virtual double Cosh(double x)
        {
            return Math.Cosh(x);
        }

        float IMathApi.Tanhf(float x) { return Tanhf(x); }
        protected virtual float Tanhf(float x)
        {
            return (float)Math.Tanh(x);
        }

        double IMathApi.Tanh(double x) { return Tanh(x); }
        protected virtual double Tanh(double x)
        {
            return Math.Tanh(x);
        }

        float IMathApi.Asinhf(float x) { return Asinhf(x); }
        protected virtual float Asinhf(float x)
        {
            throw new NotImplementedException();
        }

        double IMathApi.Asinh(double x) { return Asinh(x); }
        protected virtual double Asinh(double x)
        {
            throw new NotImplementedException();
        }

        float IMathApi.Acoshf(float x) { return Acoshf(x); }
        protected virtual float Acoshf(float x)
        {
            throw new NotImplementedException();
        }

        double IMathApi.Acosh(double x) { return Acosh(x); }
        protected virtual double Acosh(double x)
        {
            throw new NotImplementedException();
        }

        float IMathApi.Atanhf(float x) { return Atanhf(x); }
        protected virtual float Atanhf(float x)
        {
            throw new NotImplementedException();
        }

        double IMathApi.Atanh(double x) { return Atanh(x); }
        protected virtual double Atanh(double x)
        {
            throw new NotImplementedException();
        }

        float IMathApi.Floorf(float x) { return Floorf(x); }
        protected virtual float Floorf(float x)
        {
            return (float)Math.Floor(x);
        }

        double IMathApi.Floor(double x) { return Floor(x); }
        protected virtual double Floor(double x)
        {
            return Math.Floor(x);
        }

        float IMathApi.Ceilf(float x) { return Ceilf(x); }
        protected virtual float Ceilf(float x)
        {
            return (float)Math.Ceiling(x);
        }

        double IMathApi.Ceil(double x) { return Ceil(x); }
        protected virtual double Ceil(double x)
        {
            return Math.Ceiling(x);
        }

        float IMathApi.Truncf(float x) { return Truncf(x); }
        protected virtual float Truncf(float x)
        {
            return (float)Math.Truncate(x);
        }

        double IMathApi.Trunc(double x) { return Trunc(x); }
        protected virtual double Trunc(double x)
        {
            return Math.Truncate(x);
        }

        float IMathApi.Roundf(float x) { return Roundf(x); }
        protected virtual float Roundf(float x)
        {
            return (float)Math.Round(x);
        }

        double IMathApi.Round(double x) { return Round(x); }
        protected virtual double Round(double x)
        {
            return Math.Round(x);
        }

        bool IMathApi.IsInff(float x) { return IsInff(x); }
        protected virtual bool IsInff(float x)
        {
            return float.IsInfinity(x);
        }

        bool IMathApi.IsInf(double x) { return IsInf(x); }
        protected virtual bool IsInf(double x)
        {
            return double.IsInfinity(x);
        }

        bool IMathApi.IsNanf(float x) { return IsNanf(x); }
        protected virtual bool IsNanf(float x)
        {
            return float.IsNaN(x);
        }

        bool IMathApi.IsNan(double x) { return IsNan(x); }
        protected virtual bool IsNan(double x)
        {
            return double.IsNaN(x);
        }

        bool IMathApi.IsFinitef(float x) { return IsFinitef(x); }
        protected virtual bool IsFinitef(float x)
        {
            return !IsInff(x);
        }

        bool IMathApi.IsFinite(double x) { return IsFinite(x); }
        protected virtual bool IsFinite(double x)
        {
            return !IsInf(x);
        }
    }
}