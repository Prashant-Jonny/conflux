using System.Diagnostics;
using System.Runtime.CompilerServices;
using Libptx.Bindings;

namespace Conflux.Runtime.Cuda.Api
{
    [DebuggerNonUserCode]
    internal static partial class Ctm
    {
        [Ptx("fma.rn.f64 %, %x, %y, %z"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Fma(double x, double y, double z);

        [Ptx("mad.rn.f32 %, %x, %y, %z"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Madf(float x, float y, float z);

        [Ptx("mad.rn.f64 %, %x, %y, %z"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Mad(double x, double y, double z);

        [Ptx("abs.s32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static int Abs(int x);

        [Ptx("abs.s64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static long Labs(long x);

        [Ptx("abs.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Fabsf(float x);

        [Ptx("abs.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Fabs(double x);

        [Ptx("min.s32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static int Min(int x, int y);

        [Ptx("min.u32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static uint Umin(uint x, uint y);

        [Ptx("min.s64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static long Lmin(long x, long y);

        [Ptx("min.u64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static ulong Ulmin(ulong x, ulong y);

        [Ptx("min.f32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Fminf(float x, float y);

        [Ptx("min.f64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Fmin(double x, double y);

        [Ptx("max.s32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static int Max(int x, int y);

        [Ptx("max.u32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static uint Umax(uint x, uint y);

        [Ptx("max.s64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static long Lmax(long x, long y);

        [Ptx("max.u64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static ulong Ulmax(ulong x, ulong y);

        [Ptx("max.f32 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Fmaxf(float x, float y);

        [Ptx("max.f64 %, %x, %y"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Fmax(double x, double y);

        [Ptx("sqrt.approx.ftz.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Sqrtf(float x);

        [Ptx("sqrt.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Sqrt(double x);

        [Ptx("rsqrt.approx.ftz.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Rsqrtf(float x);

        [Ptx("rsqrt.approx.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Rsqrt(double x);

        [Ptx("ex2.approx.ftz.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Exp2f(float x);

        [Ptx("lg2.approx.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Log2f(float x);

        [Ptx("sin.approx.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Sinf(float x);

        [Ptx("cos.approx.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Cosf(float x);

        [Ptx("cvt.rn.rmi.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Floorf(float x);

        [Ptx("cvt.rn.rmi.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Floor(double x);

        [Ptx("cvt.rn.rpi.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Ceilf(float x);

        [Ptx("cvt.rn.rpi.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Ceil(double x);

        [Ptx("cvt.rn.rzi.f32 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static float Truncf(float x);

        [Ptx("cvt.rn.rzi.f64 %, %x"), MethodImpl(MethodImplOptions.InternalCall)]
        extern public static double Trunc(double x);
    }
}
