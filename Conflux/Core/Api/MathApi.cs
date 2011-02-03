using System.Diagnostics;
using Conflux.Core.Api.Registry;
using Conflux.Runtime.Common.Registry;

namespace Conflux.Core.Api
{
    [Api, DebuggerNonUserCode]
    public static class MathApi
    {
        public const float Ef = (float)System.Math.E;
        public const double E = System.Math.E;
        public const float PIf = (float)System.Math.PI;
        public const double PI = System.Math.PI;
        public const float PosInff = float.PositiveInfinity;
        public const double PosInf = double.PositiveInfinity;
        public const float NegInff = float.NegativeInfinity;
        public const double NegInf = double.NegativeInfinity;

        public static float Fmaf(float x, float y, float z)
        {
            return Runtimes.ActiveApi.Fmaf(x, y, z);
        }

        public static double Fma(double x, double y, double z)
        {
            return Runtimes.ActiveApi.Fma(x, y, z);
        }

        public static float Madf(float x, float y, float z)
        {
            return Runtimes.ActiveApi.Madf(x, y, z);
        }

        public static double Mad(double x, double y, double z)
        {
            return Runtimes.ActiveApi.Mad(x, y, z);
        }

        public static int Abs(int x)
        {
            return Runtimes.ActiveApi.Abs(x);
        }

        public static long Labs(long x)
        {
            return Runtimes.ActiveApi.Labs(x);
        }

        public static float Fabsf(float x)
        {
            return Runtimes.ActiveApi.Fabsf(x);
        }

        public static double Fabs(double x)
        {
            return Runtimes.ActiveApi.Fabs(x);
        }

        public static int Min(int x, int y)
        {
            return Runtimes.ActiveApi.Min(x, y);
        }

        public static uint Umin(uint x, uint y)
        {
            return Runtimes.ActiveApi.Umin(x, y);
        }

        public static long Lmin(long x, long y)
        {
            return Runtimes.ActiveApi.Lmin(x, y);
        }

        public static ulong Ulmin(ulong x, ulong y)
        {
            return Runtimes.ActiveApi.Ulmin(x, y);
        }

        public static float Fminf(float x, float y)
        {
            return Runtimes.ActiveApi.Fminf(x, y);
        }

        public static double Fmin(double x, double y)
        {
            return Runtimes.ActiveApi.Fmin(x, y);
        }

        public static int Max(int x, int y)
        {
            return Runtimes.ActiveApi.Max(x, y);
        }

        public static uint Umax(uint x, uint y)
        {
            return Runtimes.ActiveApi.Umax(x, y);
        }

        public static long Lmax(long x, long y)
        {
            return Runtimes.ActiveApi.Lmax(x, y);
        }

        public static ulong Ulmax(ulong x, ulong y)
        {
            return Runtimes.ActiveApi.Ulmax(x, y);
        }

        public static float Fmaxf(float x, float y)
        {
            return Runtimes.ActiveApi.Fmaxf(x, y);
        }

        public static double Fmax(double x, double y)
        {
            return Runtimes.ActiveApi.Fmax(x, y);
        }

        public static float Sqrtf(float x)
        {
            return Runtimes.ActiveApi.Sqrtf(x);
        }

        public static double Sqrt(double x)
        {
            return Runtimes.ActiveApi.Sqrt(x);
        }

        public static float Rsqrtf(float x)
        {
            return Runtimes.ActiveApi.Rsqrtf(x);
        }

        public static double Rsqrt(double x)
        {
            return Runtimes.ActiveApi.Rsqrt(x);
        }

        public static float Cbrtf(float x)
        {
            return Runtimes.ActiveApi.Cbrtf(x);
        }

        public static double Cbrt(double x)
        {
            return Runtimes.ActiveApi.Cbrt(x);
        }

        public static float Rcbrtf(float x)
        {
            return Runtimes.ActiveApi.Rcbrtf(x);
        }

        public static double Rcbrt(double x)
        {
            return Runtimes.ActiveApi.Rcbrt(x);
        }

        public static float Powf(float x, float y)
        {
            return Runtimes.ActiveApi.Powf(x, y);
        }

        public static double Pow(double x, double y)
        {
            return Runtimes.ActiveApi.Pow(x, y);
        }

        public static float Exp2f(float x)
        {
            return Runtimes.ActiveApi.Exp2f(x);
        }

        public static double Exp2(double x)
        {
            return Runtimes.ActiveApi.Exp2(x);
        }

        public static float Exp10f(float x)
        {
            return Runtimes.ActiveApi.Exp10f(x);
        }

        public static double Exp10(double x)
        {
            return Runtimes.ActiveApi.Exp10(x);
        }

        public static float Expm1f(float x)
        {
            return Runtimes.ActiveApi.Expm1f(x);
        }

        public static double Expm1(double x)
        {
            return Runtimes.ActiveApi.Expm1(x);
        }

        public static float Expf(float x)
        {
            return Runtimes.ActiveApi.Expf(x);
        }

        public static double Exp(double x)
        {
            return Runtimes.ActiveApi.Exp(x);
        }

        public static float Logf(float x)
        {
            return Runtimes.ActiveApi.Logf(x);
        }

        public static double Log(double x)
        {
            return Runtimes.ActiveApi.Log(x);
        }

        public static float Log2f(float x)
        {
            return Runtimes.ActiveApi.Log2f(x);
        }

        public static double Log2(double x)
        {
            return Runtimes.ActiveApi.Log2(x);
        }

        public static float Log10f(float x)
        {
            return Runtimes.ActiveApi.Log10f(x);
        }

        public static double Log10(double x)
        {
            return Runtimes.ActiveApi.Log10(x);
        }

        public static float Log1pf(float x)
        {
            return Runtimes.ActiveApi.Log1pf(x);
        }

        public static double Log1p(double x)
        {
            return Runtimes.ActiveApi.Log1p(x);
        }

        public static float Logf(float x, float @base)
        {
            return Runtimes.ActiveApi.Logf(x, @base);
        }

        public static double Log(double x, double @base)
        {
            return Runtimes.ActiveApi.Log(x, @base);
        }

        public static float Sinf(float x)
        {
            return Runtimes.ActiveApi.Sinf(x);
        }

        public static double Sin(double x)
        {
            return Runtimes.ActiveApi.Sin(x);
        }

        public static float Sinpif(float x)
        {
            return Runtimes.ActiveApi.Sinpif(x);
        }

        public static double Sinpi(double x)
        {
            return Runtimes.ActiveApi.Sinpi(x);
        }

        public static float Cosf(float x)
        {
            return Runtimes.ActiveApi.Cosf(x);
        }

        public static double Cos(double x)
        {
            return Runtimes.ActiveApi.Cos(x);
        }

        public static void Sincosf(float x, out float sin, out float cos)
        {
            Runtimes.ActiveApi.Sincosf(x, out sin, out cos);
        }

        public static void Sincos(double x, out double sin, out double cos)
        {
            Runtimes.ActiveApi.Sincos(x, out sin, out cos);
        }

        public static float Tanf(float x)
        {
            return Runtimes.ActiveApi.Tanf(x);
        }

        public static double Tan(double x)
        {
            return Runtimes.ActiveApi.Tan(x);
        }

        public static float Asinf(float x)
        {
            return Runtimes.ActiveApi.Asinf(x);
        }

        public static double Asin(double x)
        {
            return Runtimes.ActiveApi.Asin(x);
        }

        public static float Acosf(float x)
        {
            return Runtimes.ActiveApi.Acosf(x);
        }

        public static double Acos(double x)
        {
            return Runtimes.ActiveApi.Acos(x);
        }

        public static float Atanf(float x)
        {
            return Runtimes.ActiveApi.Atanf(x);
        }

        public static double Atan(double x)
        {
            return Runtimes.ActiveApi.Atan(x);
        }

        public static float Atan2f(float y, float x)
        {
            return Runtimes.ActiveApi.Atan2f(y, x);
        }

        public static double Atan2(double y, double x)
        {
            return Runtimes.ActiveApi.Atan2(y, x);
        }

        public static float Sinhf(float x)
        {
            return Runtimes.ActiveApi.Atanf(x);
        }

        public static double Sinh(double x)
        {
            return Runtimes.ActiveApi.Sinh(x);
        }

        public static float Coshf(float x)
        {
            return Runtimes.ActiveApi.Coshf(x);
        }

        public static double Cosh(double x)
        {
            return Runtimes.ActiveApi.Cosh(x);
        }

        public static float Tanhf(float x)
        {
            return Runtimes.ActiveApi.Tanhf(x);
        }

        public static double Tanh(double x)
        {
            return Runtimes.ActiveApi.Tanh(x);
        }

        public static float Asinhf(float x)
        {
            return Runtimes.ActiveApi.Asinhf(x);
        }

        public static double Asinh(double x)
        {
            return Runtimes.ActiveApi.Asinh(x);
        }

        public static float Acoshf(float x)
        {
            return Runtimes.ActiveApi.Acoshf(x);
        }

        public static double Acosh(double x)
        {
            return Runtimes.ActiveApi.Acosh(x);
        }

        public static float Atanhf(float x)
        {
            return Runtimes.ActiveApi.Atanhf(x);
        }

        public static double Atanh(double x)
        {
            return Runtimes.ActiveApi.Atanh(x);
        }

        public static float Floorf(float x)
        {
            return Runtimes.ActiveApi.Floorf(x);
        }

        public static double Floor(double x)
        {
            return Runtimes.ActiveApi.Floor(x);
        }

        public static float Ceilf(float x)
        {
            return Runtimes.ActiveApi.Ceilf(x);
        }

        public static double Ceil(double x)
        {
            return Runtimes.ActiveApi.Ceil(x);
        }

        public static float Truncf(float x)
        {
            return Runtimes.ActiveApi.Truncf(x);
        }

        public static double Trunc(double x)
        {
            return Runtimes.ActiveApi.Trunc(x);
        }

        public static float Roundf(float x)
        {
            return Runtimes.ActiveApi.Roundf(x);
        }

        public static double Round(double x)
        {
            return Runtimes.ActiveApi.Round(x);
        }

        public static bool IsInff(float x)
        {
            return Runtimes.ActiveApi.IsInff(x);
        }

        public static bool IsInf(double x)
        {
            return Runtimes.ActiveApi.IsInf(x);
        }

        public static bool IsNanf(float x)
        {
            return Runtimes.ActiveApi.IsNanf(x);
        }

        public static bool IsNan(double x)
        {
            return Runtimes.ActiveApi.IsNan(x);
        }

        public static bool IsFinitef(float x)
        {
            return Runtimes.ActiveApi.IsFinitef(x);
        }

        public static bool IsFinite(double x)
        {
            return Runtimes.ActiveApi.IsFinite(x);
        }
    }
}