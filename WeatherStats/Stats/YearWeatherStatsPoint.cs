using System.Text.Json.Serialization;

namespace Pmad.WeatherStats.Stats
{
    /// <summary>
    /// Associates a geographic point (latitude/longitude) with its full-year monthly weather statistics.
    /// </summary>
    public sealed class YearWeatherStatsPoint
    {
        /// <summary>
        /// Initializes a new instance of <see cref="YearWeatherStatsPoint"/>.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees (positive = North).</param>
        /// <param name="longitude">Longitude in decimal degrees (positive = East).</param>
        /// <param name="months">Array of 12 monthly statistics, ordered January through December.</param>
        [JsonConstructor]
        public YearWeatherStatsPoint(float latitude, float longitude, MonthWeatherStatsData[] months)
        {
            Latitude = latitude;
            Longitude = longitude;
            Months = months;
        }

        /// <summary>Gets the latitude in decimal degrees (positive = North).</summary>
        [JsonPropertyName("lat")]
        public float Latitude { get; }

        /// <summary>Gets the longitude in decimal degrees (positive = East).</summary>
        [JsonPropertyName("lon")]
        public float Longitude { get; }

        /// <summary>Gets the 12 monthly weather statistics, ordered January through December.</summary>
        [JsonPropertyName("m")]
        public MonthWeatherStatsData[] Months { get; }
    }
}
