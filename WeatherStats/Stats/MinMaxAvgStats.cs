using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Pmad.WeatherStats.Stats
{
    /// <summary>
    /// Holds statistics (min, average, max) where each of those is itself a <see cref="MinMaxAvg"/>.
    /// Typically used for variables like temperature or wind speed where daily min/avg/max values
    /// are aggregated across multiple days.
    /// </summary>
    [DebuggerDisplay("{Avg.Avg} ({Min.Avg} to {Max.Avg})")]
    public sealed class MinMaxAvgStats
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MinMaxAvgStats"/>.
        /// </summary>
        /// <param name="min">Statistics of the daily minimum values.</param>
        /// <param name="avg">Statistics of the daily average values.</param>
        /// <param name="max">Statistics of the daily maximum values.</param>
        [JsonConstructor]
        public MinMaxAvgStats(MinMaxAvg min, MinMaxAvg avg, MinMaxAvg max)
        {
            Min = min;
            Avg = avg;
            Max = max;
        }

        /// <summary>Gets the statistics of daily minimum values.</summary>
        [JsonPropertyName("i")]
        public MinMaxAvg Min { get; }

        /// <summary>Gets the statistics of daily average values.</summary>
        [JsonPropertyName("v")]
        public MinMaxAvg Avg { get; }

        /// <summary>Gets the statistics of daily maximum values.</summary>
        [JsonPropertyName("a")]
        public MinMaxAvg Max { get; }

        /// <summary>
        /// Creates a <see cref="MinMaxAvgStats"/> from a sequence of <see cref="MinMaxAvg"/> values.
        /// </summary>
        /// <param name="values">Per-day <see cref="MinMaxAvg"/> entries.</param>
        public static MinMaxAvgStats From(IEnumerable<MinMaxAvg> values)
        {
            return new MinMaxAvgStats(
                MinMaxAvg.From(values.Select(v => v.Min)),
                MinMaxAvg.From(values.Select(v => v.Avg)),
                MinMaxAvg.From(values.Select(v => v.Max)));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxAvgStats"/> from a flat array of values by splitting it into
        /// daily slices and computing per-slice <see cref="MinMaxAvg"/> entries.
        /// </summary>
        /// <param name="values">Flat array of values (e.g. hourly readings).</param>
        /// <param name="slice">Number of values per day. Defaults to 24.</param>
        public static MinMaxAvgStats From(float[] values, int slice = 24)
        {
            var result = new List<MinMaxAvg>(31);
            for (int i = 0; i < values.Length; i += slice)
            {
                result.Add(MinMaxAvg.From(values, i, slice));
            }
            return From(result);
        }

        internal static MinMaxAvgStats Average(IEnumerable<MinMaxAvgStats> enumerable)
        {
            return new MinMaxAvgStats(
                MinMaxAvg.Average(enumerable.Select(i => i.Min)),
                MinMaxAvg.Average(enumerable.Select(i => i.Avg)),
                MinMaxAvg.Average(enumerable.Select(i => i.Max)));
        }
    }
}
