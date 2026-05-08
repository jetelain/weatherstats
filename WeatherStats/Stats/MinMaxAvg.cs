using System.Diagnostics;
using System.Text.Json.Serialization;

namespace WeatherStats.Stats
{
    /// <summary>
    /// Holds the minimum, average, and maximum of a set of float values.
    /// </summary>
    [DebuggerDisplay("{Avg} ({Min} to {Max})")]
    public sealed class MinMaxAvg
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MinMaxAvg"/>.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="avg">The average value.</param>
        /// <param name="max">The maximum value.</param>
        [JsonConstructor]
        public MinMaxAvg(float min, float avg, float max)
        {
            Min = min;
            Avg = avg;
            Max = max;
        }

        /// <summary>Gets the minimum value.</summary>
        [JsonPropertyName("i")]
        public float Min { get; }

        /// <summary>Gets the average value.</summary>
        [JsonPropertyName("v")]
        public float Avg { get; }

        /// <summary>Gets the maximum value.</summary>
        [JsonPropertyName("a")]
        public float Max { get; }

        /// <summary>
        /// Creates a <see cref="MinMaxAvg"/> from a slice of an array.
        /// </summary>
        /// <param name="values">Source array.</param>
        /// <param name="start">Start index of the slice.</param>
        /// <param name="length">Length of the slice.</param>
        public static MinMaxAvg From(float[] values, int start, int length)
        {
            return From(new ArraySegment<float>(values, start, length));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxAvg"/> from a sequence of values, ignoring <see cref="float.NaN"/>.
        /// </summary>
        /// <param name="values">Input values.</param>
        public static MinMaxAvg From(IEnumerable<float> values)
        {
            var count = 0;
            var total = 0d;
            var min = 0f;
            var max = 0f;
            foreach (var v in values)
            {
                if (!float.IsNaN(v))
                {
                    if (count == 0)
                    {
                        total = v;
                        min = v;
                        max = v;
                    }
                    else
                    {
                        total += v;
                        min = MathF.Min(min, v);
                        max = MathF.Max(max, v);
                    }
                    count++;
                }
            }
            return new MinMaxAvg(min, (float)(total / count), max);
        }

        /// <summary>
        /// Computes a <see cref="MinMaxAvg"/> by averaging the min, avg, and max of multiple instances.
        /// </summary>
        /// <param name="enumerable">Instances to average.</param>
        public static MinMaxAvg Average(IEnumerable<MinMaxAvg> enumerable)
        {
            return new MinMaxAvg(
                enumerable.Select(i => i.Min).Average(),
                enumerable.Select(i => i.Avg).Average(),
                enumerable.Select(i => i.Max).Average());
        }
    }
}
