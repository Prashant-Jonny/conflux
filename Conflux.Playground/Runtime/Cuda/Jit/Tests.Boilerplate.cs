using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Conflux.Core.Configuration.Cuda;
using Libcuda.DataTypes;
using Conflux.Runtime.Common.Registry;
using Conflux.Runtime.Cuda;
using Conflux.Runtime.Cuda.Jit;
using XenoGears.Playground.Framework;
using XenoGears.Streams;
using XenoGears.Traits.Dumpable;
using XenoGears.Functional;
using XenoGears.Strings;
using NUnit.Framework;
using XenoGears;

namespace Conflux.Playground.Runtime.Cuda.Jit
{
    public abstract class Tests : BaseTests
    {
        protected void TestKernelCrosscompilation(Type t_kernel)
        {
            var cfg = new CudaConfig{BlockDim = new dim3(16, 16, 1)};
            cfg.Codebase.OptIn(t => t.Assembly.GetName().Name == "Conflux.Playground");

            using (Runtimes.Activate(new CudaRuntime(cfg, t_kernel)))
            {
                var result = JitCompiler.DoCompile(cfg, t_kernel);
                var s_ptx_actual = result.Ptx;
                var s_hir_actual = result.Hir.DumpAsText();

                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                var @namespace = MethodInfo.GetCurrentMethod().DeclaringType.Namespace;
                @namespace += ".Reference.";
                var ptx_fileName = asm.GetManifestResourceNames().SingleOrDefault2(
                    n => String.Compare(n, @namespace + t_kernel.Name + ".ptx", true) == 0);
                var hir_fileName = asm.GetManifestResourceNames().SingleOrDefault2(
                    n => String.Compare(n, @namespace + t_kernel.Name + ".hir", true) == 0);

                Verify(s_ptx_actual, ptx_fileName, "crosscompiled PTX", t_kernel);
                Verify(s_hir_actual, hir_fileName, "crosscompiled HIR", t_kernel);
            }
        }

        private void Verify(String s_actual, String fileName, String what, Type t_kernel)
        {
            var success = false;
            String failMsg = null;
            if (fileName != null)
            {
                var asm = MethodInfo.GetCurrentMethod().DeclaringType.Assembly;
                var s_reference = asm.ReadText(fileName);

                if (s_reference.IsEmpty())
                {
                    Log.WriteLine(s_actual);

                    Assert.Fail(String.Format(
                        "Reference " + what + " for the kernel '{1}' is empty.{0}" +
                        "Please, verify the trace dumped above and put in into the resource file.",
                        Environment.NewLine, t_kernel.GetCSharpRef(ToCSharpOptions.Informative)));
                }
                else
                {
                    var expected = s_reference.SplitLines();
                    var actual = s_actual.SplitLines();
                    if (expected.Count() != actual.Count())
                    {
                        failMsg = String.Format(
                            "Number of lines doesn't match. Expected {0}, actually found {1}" + Environment.NewLine,
                            expected.Count(), actual.Count());
                    }
                    else
                    {
                        success = expected.Zip(actual).SkipWhile((t, i) =>
                        {
                            if (t.Item1 != t.Item2)
                            {
                                var maxLines = Math.Max(actual.Count(), expected.Count());
                                var maxDigits = (int)Math.Floor(Math.Log10(maxLines)) + 1;
                                failMsg = String.Format(
                                    "Line {1} (starting from 1) doesn't match.{0}{4}{2}{0}{5}{3}{0}",
                                    Environment.NewLine, i + 1,
                                    t.Item1.Replace(" ", "·"), t.Item2.Replace(" ", "·"),
                                    "E:".PadRight(maxDigits + 3), "A:".PadRight(maxDigits + 3));
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }).IsEmpty();
                    }

                    if (!success)
                    {
                        Log.WriteLine(t_kernel.GetCSharpRef(ToCSharpOptions.Informative));
                        Log.WriteLine(String.Empty);

                        var maxLines = Math.Max(actual.Count(), expected.Count());
                        var maxDigits = (int)Math.Floor(Math.Log10(maxLines)) + 1;
                        var maxActual = Math.Max(actual.MaxOrDefault(line => line.Length), "Actual".Length);
                        var maxExpected = Math.Max(expected.MaxOrDefault(line => line.Length), "Expected".Length);
                        var total = maxDigits + 3 + maxActual + 3 + maxExpected;

                        Log.WriteLine(String.Format("{0} | {1} | {2}",
                            "N".PadRight(maxDigits),
                            "Actual".PadRight(maxActual),
                            "Expected".PadRight(maxExpected)));
                        Log.WriteLine(total.Times("-"));

                        0.UpTo(maxLines - 1).ForEach(i =>
                        {
                            var l_actual = actual.ElementAtOrDefault(i, String.Empty);
                            var l_expected = expected.ElementAtOrDefault(i, String.Empty);
                            Log.WriteLine(String.Format("{0} | {1} | {2}",
                                i.ToString().PadLeft(maxDigits),
                                l_actual.PadRight(maxActual),
                                l_expected.PadRight(maxExpected)));
                        });

                        Log.WriteLine(String.Empty);
                        Log.WriteLine(failMsg);
                        Log.WriteLine(String.Empty);
                        Assert.Fail(what.Capitalize() + " doesn't match reference " + what + ".");
                    }
                }
            }
            else
            {
                Log.WriteLine(s_actual);

                Assert.Fail(String.Format(
                    "Couldn't find a file in resource that contains reference " + what + " for the kernel '{1}'.{0}" +
                    "Please, verify the trace dumped above and put in into the '{2}' file under the Reference folder next to tests.{0}" +
                    "Also be sure not to forget to select build action 'Embedded Resource' in file properties widget.",
                    Environment.NewLine, t_kernel.GetCSharpRef(ToCSharpOptions.Informative), fileName));
            }
        }
    }
}
