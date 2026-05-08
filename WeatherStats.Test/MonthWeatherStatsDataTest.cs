using Pmad.WeatherStats.Stats;

namespace Pmad.WeatherStats.Test
{
    public class MonthWeatherStatsDataTest
    {
        private static MonthWeatherStatsData MakeData(float baseValue)
        {
            var mma = new MinMaxAvg(baseValue, baseValue, baseValue);
            var mmas = new MinMaxAvgStats(mma, mma, mma);
            var wind = new WindDirectionStats(new float[8], new float[8]);
            return new MonthWeatherStatsData(mma, mmas, mmas, wind);
        }

        [Fact]
        public void Average_ProducesAverageOfTwoEntries()
        {
            var a = MakeData(2f);
            var b = MakeData(4f);
            var avg = MonthWeatherStatsData.Average(new[] { a, b });
            Assert.Equal(3f, avg.Humidity.Avg);
            Assert.Equal(3f, avg.Temperature.Avg.Avg);
            Assert.Equal(3f, avg.WindSpeed.Avg.Avg);
        }

        [Fact]
        public void From_BuildsFromRawArrays()
        {
            var humidity = new float[] { 50f, 60f, 70f };
            var temperature = Enumerable.Repeat(20f, 48).ToArray();
            var windSpeed = Enumerable.Repeat(5f, 48).ToArray();
            var windDir = Enumerable.Repeat(WindDirection8.North, 48).ToArray();

            var data = MonthWeatherStatsData.From(humidity, temperature, windSpeed, windDir);

            Assert.Equal(50f, data.Humidity.Min);
            Assert.Equal(70f, data.Humidity.Max);
            Assert.Equal(20f, data.Temperature.Avg.Avg);
            Assert.Equal(WindDirection8.North, data.WindDirection.Prevailing);
        }
    }
}
