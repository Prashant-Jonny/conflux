using Conflux.Core.Api.Registry;

namespace Conflux.Core.Api
{
    // note. the APIs below use hungarian notation of names instead of plain overloading
    // that's done on purpose, since numeric coercions of C# 
    // significantly differ from numeric coercions of C/C++
    // since at the moment I've got no time to think about a robust solution
    // here's what we've got - quick, dirty and, by the way, conformant to CUDA style
    // also see C:\Program Files\CUDA\Toolkit\include\math_functions.h

    // note. there's a protosolution, but it has its shortcomings, so it needs additional thinking
    // we might neither define multiple hungarian names, neither define multiple overloads
    // but rather define a single signature, e.g. "Number Min(Number x, Number y)":
    // 1) Number would be an auxiliary type that just has implicit conversions from/to all numerics
    // 2) CPU implementation requires extensive codegen to work fast, but that's possible in principle
    // 3) GPU implementation can analyze implicit casts emitted by CSC and think what to do with them
    // 4) Later we can introduce Integer and Float numbers to differentiate between int- and float- apis
    // 5) Later we can introduce NumberX vectors or even try to unify those into the Number itself

    // note. the solution described above looks VERY awesome (thanks to Andrey for suggesting it)
    // however, I'm afraid that we might have problems with the return type
    // from the other side, if we overload arithmetic operators
    // then surrogate returns from api calls can be used in further calculations
    // or we could rely on CSC to insert correct implicit coercions that won't break the semantics
    // anyways, this needs: 1) time to experiment, 2) quite a bit of time to implement
    // so for now I mark this idea as promising but have to defer it

    [Api]
    public interface IMathApi
    {
        float Fmaf(float x, float y, float z);
        double Fma(double x, double y, double z);
        float Madf(float x, float y, float z);
        double Mad(double x, double y, double z);

        int Abs(int x);
        long Labs(long x);
        float Fabsf(float x);
        double Fabs(double x);

        int Min(int x, int y);
        uint Umin(uint x, uint y);
        long Lmin(long x, long y);
        ulong Ulmin(ulong x, ulong y);
        float Fminf(float x, float y);
        double Fmin(double x, double y);

        int Max(int x, int y);
        uint Umax(uint x, uint y);
        long Lmax(long x, long y);
        ulong Ulmax(ulong x, ulong y);
        float Fmaxf(float x, float y);
        double Fmax(double x, double y);

        float Sqrtf(float x);
        double Sqrt(double x);
        float Rsqrtf(float x);
        double Rsqrt(double x);
        float Cbrtf(float x);
        double Cbrt(double x);
        float Rcbrtf(float x);
        double Rcbrt(double x);
        float Powf(float x, float y);
        double Pow(double x, double y);

        float Exp2f(float x);
        double Exp2(double x);
        float Exp10f(float x);
        double Exp10(double x);
        float Expm1f(float x);
        double Expm1(double x);
        float Expf(float x);
        double Exp(double x);

        float Logf(float x);
        double Log(double x);
        float Log2f(float x);
        double Log2(double x);
        float Log10f(float x);
        double Log10(double x);
        float Log1pf(float x);
        double Log1p(double x);
        float Logf(float x, float @base);
        double Log(double x, double @base);

        float Sinf(float x);
        double Sin(double x);
        float Sinpif(float x);
        double Sinpi(double x);
        float Cosf(float x);
        double Cos(double x);
        void Sincosf(float x, out float sin, out float cos);
        void Sincos(double x, out double sin, out double cos);
        float Tanf(float x);
        double Tan(double x);

        float Asinf(float x);
        double Asin(double x);
        float Acosf(float x);
        double Acos(double x);
        float Atanf(float x);
        double Atan(double x);
        float Atan2f(float y, float x);
        double Atan2(double y, double x);

        float Sinhf(float x);
        double Sinh(double x);
        float Coshf(float x);
        double Cosh(double x);
        float Tanhf(float x);
        double Tanh(double x);
        float Asinhf(float x);
        double Asinh(double x);
        float Acoshf(float x);
        double Acosh(double x);
        float Atanhf(float x);
        double Atanh(double x);

        float Floorf(float x);
        double Floor(double x);
        float Ceilf(float x);
        double Ceil(double x);
        float Truncf(float x);
        double Trunc(double x);
        float Roundf(float x);
        double Round(double x);

        bool IsInff(float x);
        bool IsInf(double x);
        bool IsNanf(float x);
        bool IsNan(double x);
        bool IsFinitef(float x);
        bool IsFinite(double x);
    }
}