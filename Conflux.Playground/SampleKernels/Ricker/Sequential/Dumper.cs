using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Conflux.Playground.SampleKernels.Matmul.Domain;
using Conflux.Playground.SampleKernels.Ricker.Domain;
using XenoGears.Functional;

namespace Conflux.Playground.SampleKernels.Ricker.Sequential
{
    public class Dumper : IDisposable
    {
        public const String PathToTau = @"d:\aero\tau\";
        public const String PathToUx = @"d:\aero\ux\";
        public const String PathToUz = @"d:\aero\uz\";

        private readonly List<Matrix<Cell>> _log = new List<Matrix<Cell>>();
        public void Dump(Matrix<Cell> matrix) { _log.Add(new Matrix<Cell>(matrix.Height, matrix.Width, (i, j) => matrix[i, j])); }

        public void Dispose()
        {
            if (_log.IsEmpty())
            {
                return;
            }
            else
            {
                var sample = _log.First();
                var mins = new Matrix<Cell>(sample.Height, sample.Width);
                var maxes = new Matrix<Cell>(sample.Height, sample.Width);

                var gmin = new Cell();
                var gmax = new Cell();
                for (var i = 0; i < sample.Height; ++i)
                {
                    for (var j = 0; j < sample.Width; ++j)
                    {
                        var min_tau = _log.Select(m => Math.Abs(m[i, j].Tau)).Where(val => val > 0).MinOrDefault();
                        var min_ux = _log.Select(m => Math.Abs(m[i, j].Ux)).Where(val => val > 0).MinOrDefault();
                        var min_uz = _log.Select(m => Math.Abs(m[i, j].Uz)).Where(val => val > 0).MinOrDefault();
                        var min = new Cell { Tau = min_tau, Ux = min_ux, Uz = min_uz };
                        mins[i, j] = min;

                        Func<float, float, float> minof = (v1, v2) => v1 == 0 ? v2 : v2 == 0 ? v1 : Math.Min(v1, v2);
                        gmin.Tau = minof(min_tau, gmin.Tau);
                        gmin.Ux = minof(min_ux, gmin.Ux);
                        gmin.Uz = minof(min_uz, gmin.Uz);

                        var max_tau = _log.Select(m => Math.Abs(m[i, j].Tau)).Where(val => val > 0).MaxOrDefault();
                        var max_ux = _log.Select(m => Math.Abs(m[i, j].Ux)).Where(val => val > 0).MaxOrDefault();
                        var max_uz = _log.Select(m => Math.Abs(m[i, j].Uz)).Where(val => val > 0).MaxOrDefault();
                        var max = new Cell { Tau = max_tau, Ux = max_ux, Uz = max_uz };
                        maxes[i, j] = max;

                        Func<float, float, float> maxof = (v1, v2) => v1 == 0 ? v2 : v2 == 0 ? v1 : Math.Max(v1, v2);
                        gmax.Tau = maxof(max_tau, gmax.Tau);
                        gmax.Ux = maxof(max_ux, gmax.Ux);
                        gmax.Uz = maxof(max_uz, gmax.Uz);
                    }
                }

                if (Directory.Exists(PathToTau)) Directory.Delete(PathToTau, true);
                if (Directory.Exists(PathToUx)) Directory.Delete(PathToUx, true);
                if (Directory.Exists(PathToUz)) Directory.Delete(PathToUz, true);
                Directory.CreateDirectory(PathToTau);
                Directory.CreateDirectory(PathToUx);
                Directory.CreateDirectory(PathToUz);

                var index = 0;
                foreach (var entry in _log)
                {
                    var tauBmp = new Bitmap(entry.Width, entry.Height);
                    var uxBmp = new Bitmap(entry.Width, entry.Height);
                    var uzBmp = new Bitmap(entry.Width, entry.Height);

                    for (var i = 0; i < entry.Height; ++i)
                    {
                        for (var j = 0; j < entry.Height; ++j)
                        {
                            var tauci = Rank(entry[i, j].Tau, gmax.Tau, Palette.Count);
                            var uxci = Rank(entry[i, j].Ux, gmax.Ux, Palette.Count);
                            var uzci = Rank(entry[i, j].Uz, gmax.Uz, Palette.Count);

                            tauBmp.SetPixel(j, i, Palette.ElementAt(tauci));
                            uxBmp.SetPixel(j, i, Palette.ElementAt(uxci));
                            uzBmp.SetPixel(j, i, Palette.ElementAt(uzci));
                        }
                    }

                    tauBmp.Save(PathToTau + index.ToString("0000") + ".bmp");
                    uxBmp.Save(PathToUx + index.ToString("0000") + ".bmp");
                    uzBmp.Save(PathToUz + index.ToString("0000") + ".bmp");

                    index++;
                }
            }
        }

        private static ReadOnlyCollection<Color> _palette;
        private static ReadOnlyCollection<Color> Palette
        {
            get
            {
                if (_palette == null)
                {
                    _palette = new ReadOnlyCollection<Color>(PaletteImpl.ToList());
                }

                return _palette;
            }
        }

        // as in email - obviously wrong since (0, 0, 0) is black, but not white
//        private static IEnumerable<Color> PaletteImpl
//        {
//            get
//            {
//                var curr = Color.FromArgb(255, 0, 0);
//                while (curr != Color.FromArgb(0, 0, 255))
//                {
//                    yield return curr;
//
//                    var r = curr.R;
//                    var b = curr.B;
//
//                    if (r > 0) r -= 2;
//                    if (r == 1) { r = 0; b = 1; }
//                    if (b > 0) b += 2;
//
//                    curr = Color.FromArgb(r, 0, b);
//                }
//            }
//        }

        private static IEnumerable<Color> PaletteImpl
        {
            get
            {
                Func<Color, Color, int, int, Color> gradient = (from, to, index, total) =>
                {
                    var r_step = (from.R == to.R) ? 0.0d : ((to.R - from.R) * 1.0d / (total - 1));
                    var g_step = (from.G == to.G) ? 0.0d : ((to.G - from.G) * 1.0d / (total - 1));
                    var b_step = (from.B == to.B) ? 0.0d : ((to.B - from.B) * 1.0d / (total - 1));

                    var r = (int)((float)from.R + r_step * index);
                    var g = (int)((float)from.G + g_step * index);
                    var b = (int)((float)from.B + b_step * index);
                    return Color.FromArgb(r, g, b);
                };

                const int steps = 40;
                for (var i = 0; i < steps - 1; ++i) yield return gradient(Color.Red, Color.White, i, steps);
                for (var i = 0; i < steps; ++i) yield return gradient(Color.White, Color.Blue, i, steps);
            }
        }

        private int Rank(float val, float maxOfAbs, int totalRanks)
        {
            return (int)(((val / maxOfAbs) + 1) / 2 * (totalRanks - 1));
        }
    }
}
