using System.Text.Json.Serialization;

namespace WeatherStats
{
    public class WeatherStatsPoint
    {
        [JsonConstructor]
        public WeatherStatsPoint(float lat, float lon, WeatherStatsData data) {

            Latitude = lat;
            Longitude = lon;
            Data = data;
        }

        [JsonPropertyName("lat")]
        public float Latitude { get; }

        [JsonPropertyName("lon")]
        public float Longitude { get; }

        [JsonPropertyName("d")]
        public WeatherStatsData Data { get; }
    }
}
