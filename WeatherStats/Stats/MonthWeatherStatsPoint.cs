using System.Text.Json.Serialization;

namespace WeatherStats.Stats
{
    /// <summary>
    /// Associates a geographic point (latitude/longitude) with its monthly weather statistics.
    /// </summary>
    public sealed class MonthWeatherStatsPoint
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MonthWeatherStatsPoint"/>.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees (positive = North).</param>
        /// <param name="longitude">Longitude in decimal degrees (positive = East).</param>
        /// <param name="data">Weather statistics for this point.</param>
        [JsonConstructor]
        public MonthWeatherStatsPoint(float latitude, float longitude, MonthWeatherStatsData data)
        {
            Latitude = latitude;
            Longitude = longitude;
            Data = data;
        }

        /// <summary>Gets the latitude in decimal degrees (positive = North).</summary>
        [JsonPropertyName("lat")]
        public float Latitude { get; }

        /// <summary>Gets the longitude in decimal degrees (positive = East).</summary>
        [JsonPropertyName("lon")]
        public float Longitude { get; }

        /// <summary>Gets the monthly weather statistics for this point.</summary>
        [JsonPropertyName("d")]
        public MonthWeatherStatsData Data { get; }
    }
}
