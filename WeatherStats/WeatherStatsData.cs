using System.Text.Json.Serialization;

namespace WeatherStats
{
    public class WeatherStatsData
    {
        [JsonConstructor]
        public WeatherStatsData(MinMaxAvg humidity, MinMaxAvgStats temperature, MinMaxAvgStats windSpeed, WindDirectionStats windDirection)
        {
            Humidity = humidity;
            Temperature = temperature;
            WindSpeed = windSpeed;
            WindDirection = windDirection;
        }

        [JsonPropertyName("h")]
        public MinMaxAvg Humidity { get; }

        [JsonPropertyName("t")]
        public MinMaxAvgStats Temperature { get; }

        [JsonPropertyName("ws")]
        public MinMaxAvgStats WindSpeed { get; }

        [JsonPropertyName("wd")]
        public WindDirectionStats WindDirection { get; }

        public static WeatherStatsData From(float[] humidity, float[] temperature, float[] windSpeed, WindDirection8[] windDirection)
        {
            return new WeatherStatsData(
                MinMaxAvg.From(humidity),
                MinMaxAvgStats.From(temperature),
                MinMaxAvgStats.From(windSpeed),
                WindDirectionStats.From(windSpeed, windDirection));
        }

    }
}
