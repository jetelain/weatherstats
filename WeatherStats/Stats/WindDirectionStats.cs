using System.Diagnostics;
using System.Text.Json.Serialization;

namespace WeatherStats.Stats
{
    /// <summary>
    /// Holds wind direction statistics: occurrence probability and average speed per direction.
    /// </summary>
    [DebuggerDisplay("{Prevailing}")]
    public class WindDirectionStats
    {
        private static WindDirection8[] Directions = Enum.GetValues<WindDirection8>();

        /// <summary>
        /// Initializes a new instance of <see cref="WindDirectionStats"/>.
        /// </summary>
        /// <param name="probability">
        /// Occurrence probability for each <see cref="WindDirection8"/> value (indexed by enum integer).
        /// Values should sum to 1.
        /// </param>
        /// <param name="averageSpeed">
        /// Average wind speed (m/s) for each <see cref="WindDirection8"/> value (indexed by enum integer).
        /// </param>
        [JsonConstructor]
        public WindDirectionStats(float[] probability, float[] averageSpeed)
        {
            Probability = probability;
            AverageSpeed = averageSpeed;
        }

        /// <summary>
        /// Gets the occurrence probability for each <see cref="WindDirection8"/> direction
        /// (indexed by the enum integer value). Values sum to 1.
        /// </summary>
        [JsonPropertyName("p")]
        public float[] Probability { get; }

        /// <summary>
        /// Gets the average wind speed (m/s) for each <see cref="WindDirection8"/> direction
        /// (indexed by the enum integer value).
        /// </summary>
        [JsonPropertyName("s")]
        public float[] AverageSpeed { get; }

        /// <summary>Returns the occurrence probability for the specified direction.</summary>
        /// <param name="direction">Wind direction.</param>
        public float GetProbability(WindDirection8 direction)
        {
            return Probability[(int)direction];
        }

        /// <summary>Returns the average speed (m/s) for the specified direction.</summary>
        /// <param name="direction">Wind direction.</param>
        public float GetAverageSpeed(WindDirection8 direction)
        {
            return AverageSpeed[(int)direction];
        }

        /// <summary>Gets the most frequent wind direction.</summary>
        [JsonIgnore]
        public WindDirection8 Prevailing
        {
            get
            {
                var max = Probability.Max();
                return (WindDirection8)Array.IndexOf(Probability, max);
            }
        }

        /// <summary>
        /// Computes <see cref="WindDirectionStats"/> from parallel arrays of wind speed and direction.
        /// </summary>
        /// <param name="speed">Wind speed values (m/s). Non-normal values are ignored.</param>
        /// <param name="values">Wind direction for each corresponding speed value.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="speed"/> and <paramref name="values"/> have different lengths.</exception>
        public static WindDirectionStats From(float[] speed, WindDirection8[] values)
        {
            if (values.Length != speed.Length)
            {
                throw new ArgumentException();
            }
            var count = new int[Directions.Length];
            var total = new double[Directions.Length];
            for (int t = 0; t < values.Length; t++)
            {
                if (float.IsNormal(speed[t]))
                {
                    var idx = (int)values[t];
                    count[idx]++;
                    total[idx] += speed[t];
                }
            }
            var valuesCount = count.Sum();
            return new WindDirectionStats(
                count.Select(c => (float)c / valuesCount).ToArray(),
                total.Select((c, i) => count[i] > 0 ? (float)(c / count[i]) : 0).ToArray());
        }

        internal static WindDirectionStats Average(IEnumerable<WindDirectionStats> enumerable)
        {
            return new WindDirectionStats(
                    Enumerable.Range(0, 8).Select(d => enumerable.Select(i => i.Probability[d]).Average()).ToArray(),
                    Enumerable.Range(0, 8).Select(d => enumerable.Select(i => i.AverageSpeed[d]).Average()).ToArray()
                );
        }
    }
}
