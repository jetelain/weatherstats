using System.Diagnostics;
using System.Text.Json;
using PureHDF.Filters;

namespace WeatherStatsGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            H5Filter.Register(new DeflateISALFilter());
            var fileset = new DataFiles(args.Length > 0 ? args[0] : @"c:\temp\era5");
            var mask = GenerateMask(fileset);
            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 10;
            var sw1 = Stopwatch.StartNew();
            Parallel.For(0, fileset.LatValues.Length, options, latIndex =>
            {
                var lonMak = mask[latIndex];
                if (lonMak.Length != 0)
                {
                    var worker = new Worker(fileset);
                    var result = worker.Extract((ulong)latIndex, lonMak, sw1);
                    File.WriteAllText(Path.Combine(fileset.BasePath, GetFileName(latIndex, fileset)), JsonSerializer.Serialize(result));
                }
            });
            sw1.Stop();
        }

        private static string GetFileName(int latIndex, DataFiles fileset)
        {
            var value = fileset.LatValues[latIndex];

            return FormattableString.Invariant($"stats_{(value<0?"S":"N")}{MathF.Abs(value):00.00}.json");
        }

        private static bool[][] GenerateMask(DataFiles fileset)
        {
            var maskRaw = new Dictionary<int, List<int>>();
            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("WeatherStatsGenerator.landmask.json"))
            {
                maskRaw = JsonSerializer.Deserialize<Dictionary<int, List<int>>>(stream!)!;
            }
            var items = 0;
            var mask = new bool[fileset.LatValues.Length][];
            for (var latIndex = 0; latIndex < fileset.LatValues.Length; ++latIndex)
            {
                var lat = GetRawMaskIndex(fileset.LatValues[latIndex]);
                if (maskRaw.TryGetValue(lat, out var lonMak))
                {
                    mask[latIndex] = Enumerable.Range(0, fileset.LonValues.Length).Select(lonIndex => lonMak[GetRawMaskIndex(fileset.LonValues[lonIndex])] != 0).ToArray();
                    items += mask[latIndex].Count(e => e);
                }
                else
                {
                    mask[latIndex] = new bool[0];
                }
            }
            return mask;
        }

        public static int GetRawMaskIndex(float value)
        {
            return (int)Math.Round(Math.Floor(value));
        }
    }
}