using WeatherStats.Databases;

namespace WeatherStats.Test
{
    /// <summary>
    /// Integration tests that hit the live ERA5AVG database at https://weatherdata.pmad.net/ERA5AVG/
    /// These tests require network connectivity.
    /// </summary>
    public class IntegrationTest
    {
        private const string DatabaseUrl = "https://weatherdata.pmad.net/ERA5AVG/";

        private static readonly WeatherStatsDatabase Database =
            WeatherStatsDatabase.Create(DatabaseUrl);

        [Fact]
        public async Task GetStats_Geneva_ReturnsValidData()
        {
            // Geneva, Switzerland: ~46.2°N 6.15°E
            var stats = await Database.GetStats(46.2, 6.15);

            Assert.NotNull(stats);
            Assert.Equal(12, stats!.Months.Length);
        }

        [Fact]
        public async Task GetStats_Geneva_HasReasonableTemperatures()
        {
            var stats = await Database.GetStats(46.2, 6.15);
            Assert.NotNull(stats);

            foreach (var month in stats!.Months)
            {
                // Average temperature in °C: ERA5 stores in Kelvin converted values, expect reasonable range
                Assert.True(month.Temperature.Avg.Avg > -30f && month.Temperature.Avg.Avg < 50f,
                    $"Monthly avg temperature {month.Temperature.Avg.Avg} is out of expected range");
            }
        }

        [Fact]
        public async Task GetStats_Geneva_HasValidHumidity()
        {
            var stats = await Database.GetStats(46.2, 6.15);
            Assert.NotNull(stats);

            foreach (var month in stats!.Months)
            {
                Assert.True(month.Humidity.Avg >= 0f && month.Humidity.Avg <= 100f,
                    $"Monthly avg humidity {month.Humidity.Avg} is out of [0,100] range");
            }
        }

        [Fact]
        public async Task GetStats_Geneva_HasValidWindSpeed()
        {
            var stats = await Database.GetStats(46.2, 6.15);
            Assert.NotNull(stats);

            foreach (var month in stats!.Months)
            {
                Assert.True(month.WindSpeed.Avg.Avg >= 0f,
                    $"Monthly avg wind speed {month.WindSpeed.Avg.Avg} should be non-negative");
            }
        }

        [Fact]
        public async Task GetStats_Geneva_WindDirectionProbabilitiesSumToOne()
        {
            var stats = await Database.GetStats(46.2, 6.15);
            Assert.NotNull(stats);

            foreach (var month in stats!.Months)
            {
                var sum = month.WindDirection.Probability.Sum();
                Assert.Equal(1f, sum, 4);
            }
        }

        [Fact]
        public async Task GetStats_NegativeLongitude_NewYork_ReturnsValidData()
        {
            // New York City: ~40.7°N, -74.0°W
            var stats = await Database.GetStats(40.7, -74.0);

            Assert.NotNull(stats);
            Assert.Equal(12, stats!.Months.Length);
        }

        [Fact]
        public async Task GetStats_SouthernHemisphere_Sydney_ReturnsValidData()
        {
            // Sydney, Australia: ~-33.9°S, 151.2°E
            var stats = await Database.GetStats(-33.9, 151.2);

            Assert.NotNull(stats);
            Assert.Equal(12, stats!.Months.Length);
        }

        [Fact]
        public async Task GetStats_ReturnsNull_ForOpenOceanPoint()
        {
            // Middle of Pacific Ocean
            var stats = await Database.GetStats(0.0, -160.0);

            Assert.Null(stats);
        }
    }
}
