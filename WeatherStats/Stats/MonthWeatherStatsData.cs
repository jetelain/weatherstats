using System.Text.Json.Serialization;

namespace Pmad.WeatherStats.Stats
{
    /// <summary>
    /// Weather statistics for a single calendar month at a geographic point.
    /// Aggregates humidity, temperature, wind speed, and wind direction data.
    /// </summary>
    public sealed class MonthWeatherStatsData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MonthWeatherStatsData"/>.
        /// </summary>
        /// <param name="humidity">Relative humidity statistics (%).</param>
        /// <param name="temperature">Temperature statistics (°C).</param>
        /// <param name="windSpeed">Wind speed statistics (m/s).</param>
        /// <param name="windDirection">Wind direction statistics.</param>
        [JsonConstructor]
        public MonthWeatherStatsData(MinMaxAvg humidity, MinMaxAvgStats temperature, MinMaxAvgStats windSpeed, WindDirectionStats windDirection)
        {
            Humidity = humidity;
            Temperature = temperature;
            WindSpeed = windSpeed;
            WindDirection = windDirection;
        }

        /// <summary>Gets the relative humidity statistics (%).</summary>
        [JsonPropertyName("h")]
        public MinMaxAvg Humidity { get; }

        /// <summary>Gets the temperature statistics (°C).</summary>
        [JsonPropertyName("t")]
        public MinMaxAvgStats Temperature { get; }

        /// <summary>Gets the wind speed statistics (m/s).</summary>
        [JsonPropertyName("ws")]
        public MinMaxAvgStats WindSpeed { get; }

        /// <summary>Gets the wind direction statistics.</summary>
        [JsonPropertyName("wd")]
        public WindDirectionStats WindDirection { get; }

        /// <summary>
        /// Computes the element-wise average of a collection of <see cref="MonthWeatherStatsData"/> instances.
        /// </summary>
        /// <param name="enumerable">Instances to average.</param>
        public static MonthWeatherStatsData Average(IEnumerable<MonthWeatherStatsData> enumerable)
        {
            return new MonthWeatherStatsData(
                MinMaxAvg.Average(enumerable.Select(m => m.Humidity)),
                MinMaxAvgStats.Average(enumerable.Select(m => m.Temperature)),
                MinMaxAvgStats.Average(enumerable.Select(m => m.WindSpeed)),
                WindDirectionStats.Average(enumerable.Select(m => m.WindDirection)));
        }

        /// <summary>
        /// Creates a <see cref="MonthWeatherStatsData"/> from raw per-observation arrays.
        /// </summary>
        /// <param name="humidity">Relative humidity values (%).</param>
        /// <param name="temperature">Temperature values (°C), split into 24-value daily slices.</param>
        /// <param name="windSpeed">Wind speed values (m/s), split into 24-value daily slices.</param>
        /// <param name="windDirection">Wind direction for each wind speed observation.</param>
        public static MonthWeatherStatsData From(float[] humidity, float[] temperature, float[] windSpeed, WindDirection8[] windDirection)
        {
            return new MonthWeatherStatsData(
                MinMaxAvg.From(humidity),
                MinMaxAvgStats.From(temperature),
                MinMaxAvgStats.From(windSpeed),
                WindDirectionStats.From(windSpeed, windDirection));
        }

    }
}
