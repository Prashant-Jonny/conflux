using System.Diagnostics;
using System.Runtime.CompilerServices;
using Conflux.Core.Api;

namespace Conflux.Core.Kernels
{
    [DebuggerNonUserCode]
    public abstract partial class KernelApi : IMathApi
    {
        protected IMathApi Math { get { return this; } }

        protected const float Ef = MathApi.Ef;
        protected const double E = MathApi.E;
        protected const float PIf = MathApi.PIf;
        protected const double PI = MathApi.PI;

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Fmaf(float x, float y, float z);
        float IMathApi.Fmaf(float x, float y, float z) { return Fmaf(x, y, z); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Fma(double x, double y, double z);
        double IMathApi.Fma(double x, double y, double z) { return Fma(x, y, z); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Madf(float x, float y, float z);
        float IMathApi.Madf(float x, float y, float z) { return Madf(x, y, z); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Mad(double x, double y, double z);
        double IMathApi.Mad(double x, double y, double z) { return Mad(x, y, z); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual int Abs(int x);
        int IMathApi.Abs(int x) { return Abs(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual long Labs(long x);
        long IMathApi.Labs(long x) { return Labs(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Fabsf(float x);
        float IMathApi.Fabsf(float x) { return Fabsf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Fabs(double x);
        double IMathApi.Fabs(double x) { return Fabs(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual int Min(int x, int y);
        int IMathApi.Min(int x, int y) { return Min(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual uint Umin(uint x, uint y);
        uint IMathApi.Umin(uint x, uint y) { return Umin(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual long Lmin(long x, long y);
        long IMathApi.Lmin(long x, long y) { return Lmin(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual ulong Ulmin(ulong x, ulong y);
        ulong IMathApi.Ulmin(ulong x, ulong y) { return Ulmin(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Fminf(float x, float y);
        float IMathApi.Fminf(float x, float y) { return Fminf(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Fmin(double x, double y);
        double IMathApi.Fmin(double x, double y) { return Fmin(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual int Max(int x, int y);
        int IMathApi.Max(int x, int y) { return Max(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual uint Umax(uint x, uint y);
        uint IMathApi.Umax(uint x, uint y) { return Umax(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual long Lmax(long x, long y);
        long IMathApi.Lmax(long x, long y) { return Lmax(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual ulong Ulmax(ulong x, ulong y);
        ulong IMathApi.Ulmax(ulong x, ulong y) { return Ulmax(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Fmaxf(float x, float y);
        float IMathApi.Fmaxf(float x, float y) { return Fmaxf(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Fmax(double x, double y);
        double IMathApi.Fmax(double x, double y) { return Fmax(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Sqrtf(float x);
        float IMathApi.Sqrtf(float x) { return Sqrtf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Sqrt(double x);
        double IMathApi.Sqrt(double x) { return Sqrt(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Rsqrtf(float x);
        float IMathApi.Rsqrtf(float x) { return Rsqrtf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Rsqrt(double x);
        double IMathApi.Rsqrt(double x) { return Rsqrt(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Cbrtf(float x);
        float IMathApi.Cbrtf(float x) { return Cbrtf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Cbrt(double x);
        double IMathApi.Cbrt(double x) { return Cbrt(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Rcbrtf(float x);
        float IMathApi.Rcbrtf(float x) { return Rcbrtf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Rcbrt(double x);
        double IMathApi.Rcbrt(double x) { return Rcbrt(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Powf(float x, float y);
        float IMathApi.Powf(float x, float y) { return Powf(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Pow(double x, double y);
        double IMathApi.Pow(double x, double y) { return Pow(x, y); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Exp2f(float x);
        float IMathApi.Exp2f(float x) { return Exp2f(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Exp2(double x);
        double IMathApi.Exp2(double x) { return Exp2(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Exp10f(float x);
        float IMathApi.Exp10f(float x) { return Exp10f(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Exp10(double x);
        double IMathApi.Exp10(double x) { return Exp10(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Expm1f(float x);
        float IMathApi.Expm1f(float x) { return Expm1f(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Expm1(double x);
        double IMathApi.Expm1(double x) { return Expm1(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Expf(float x);
        float IMathApi.Expf(float x) { return Expf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Exp(double x);
        double IMathApi.Exp(double x) { return Exp(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Logf(float x);
        float IMathApi.Logf(float x) { return Logf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Log(double x);
        double IMathApi.Log(double x) { return Log(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Log2f(float x);
        float IMathApi.Log2f(float x) { return Log2f(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Log2(double x);
        double IMathApi.Log2(double x) { return Log2(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Log10f(float x);
        float IMathApi.Log10f(float x) { return Log10f(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Log10(double x);
        double IMathApi.Log10(double x) { return Log10(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Log1pf(float x);
        float IMathApi.Log1pf(float x) { return Log1pf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Log1p(double x);
        double IMathApi.Log1p(double x) { return Log1p(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Logf(float x, float @base);
        float IMathApi.Logf(float x, float @base) { return Logf(x, @base); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Log(double x, double @base);
        double IMathApi.Log(double x, double @base) { return Log(x, @base); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Sinf(float x);
        float IMathApi.Sinf(float x) { return Sinf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Sin(double x);
        double IMathApi.Sin(double x) { return Sin(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Sinpif(float x);
        float IMathApi.Sinpif(float x) { return Sinpif(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Sinpi(double x);
        double IMathApi.Sinpi(double x) { return Sinpi(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Cosf(float x);
        float IMathApi.Cosf(float x) { return Cosf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Cos(double x);
        double IMathApi.Cos(double x) { return Cos(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual void Sincosf(float x, out float sin, out float cos);
        void IMathApi.Sincosf(float x, out float sin, out float cos) { Sincosf(x, out sin, out cos); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual void Sincos(double x, out double sin, out double cos);
        void IMathApi.Sincos(double x, out double sin, out double cos) { Sincos(x, out sin, out cos); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Tanf(float x);
        float IMathApi.Tanf(float x) { return Tanf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Tan(double x);
        double IMathApi.Tan(double x) { return Tan(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Asinf(float x);
        float IMathApi.Asinf(float x) { return Asinf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Asin(double x);
        double IMathApi.Asin(double x) { return Asin(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Acos(double x);
        float IMathApi.Acosf(float x) { return Acosf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Acosf(float x);
        double IMathApi.Acos(double x) { return Acos(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Atan(double x);
        float IMathApi.Atanf(float x) { return Atanf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Atanf(float x);
        double IMathApi.Atan(double x) { return Atan(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Atan2(double y, double x);
        float IMathApi.Atan2f(float y, float x) { return Atan2f(y, x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Atan2f(float y, float x);
        double IMathApi.Atan2(double y, double x) { return Atan2(y, x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Sinhf(float x);
        float IMathApi.Sinhf(float x) { return Sinhf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Sinh(double x);
        double IMathApi.Sinh(double x) { return Sinh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Coshf(float x);
        float IMathApi.Coshf(float x) { return Coshf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Cosh(double x);
        double IMathApi.Cosh(double x) { return Cosh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Tanhf(float x);
        float IMathApi.Tanhf(float x) { return Tanhf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Tanh(double x);
        double IMathApi.Tanh(double x) { return Tanh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Asinhf(float x);
        float IMathApi.Asinhf(float x) { return Asinhf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Asinh(double x);
        double IMathApi.Asinh(double x) { return Asinh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Acoshf(float x);
        float IMathApi.Acoshf(float x) { return Acoshf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Acosh(double x);
        double IMathApi.Acosh(double x) { return Acosh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Atanhf(float x);
        float IMathApi.Atanhf(float x) { return Atanhf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Atanh(double x);
        double IMathApi.Atanh(double x) { return Atanh(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Floorf(float x);
        float IMathApi.Floorf(float x) { return Floorf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Floor(double x);
        double IMathApi.Floor(double x) { return Floor(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Ceilf(float x);
        float IMathApi.Ceilf(float x) { return Ceilf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Ceil(double x);
        double IMathApi.Ceil(double x) { return Ceil(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Truncf(float x);
        float IMathApi.Truncf(float x) { return Truncf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Trunc(double x);
        double IMathApi.Trunc(double x) { return Trunc(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual float Roundf(float x);
        float IMathApi.Roundf(float x) { return Roundf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual double Round(double x);
        double IMathApi.Round(double x) { return Round(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsInff(float x);
        bool IMathApi.IsInff(float x) { return IsInff(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsInf(double x);
        bool IMathApi.IsInf(double x) { return IsInf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsNanf(float x);
        bool IMathApi.IsNanf(float x) { return IsNanf(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsNan(double x);
        bool IMathApi.IsNan(double x) { return IsNan(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsFinitef(float x);
        bool IMathApi.IsFinitef(float x) { return IsFinitef(x); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern protected virtual bool IsFinite(double x);
        bool IMathApi.IsFinite(double x) { return IsFinite(x); }
    }
}
