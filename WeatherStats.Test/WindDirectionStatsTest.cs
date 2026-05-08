using Pmad.WeatherStats.Stats;
using Pmad.WeatherStats;

namespace Pmad.WeatherStats.Test
{
    public class WindDirectionStatsTest
    {
        [Fact]
        public void From_ProbabilitiesSumToOne()
        {
            var speed = new float[] { 3f, 3f, 3f, 3f };
            var dirs = new[] { WindDirection8.North, WindDirection8.South, WindDirection8.East, WindDirection8.West };
            var stats = WindDirectionStats.From(speed, dirs);
            Assert.Equal(1f, stats.Probability.Sum(), 5);
        }

        [Fact]
        public void From_PrevailingIsCorrect()
        {
            var speed = new float[] { 3f, 3f, 3f, 3f, 3f };
            var dirs = new[]
            {
                WindDirection8.North, WindDirection8.North, WindDirection8.North,
                WindDirection8.South, WindDirection8.East
            };
            var stats = WindDirectionStats.From(speed, dirs);
            Assert.Equal(WindDirection8.North, stats.Prevailing);
        }

        [Fact]
        public void From_AverageSpeedIsCorrect()
        {
            var speed = new float[] { 2f, 4f };
            var dirs = new[] { WindDirection8.North, WindDirection8.North };
            var stats = WindDirectionStats.From(speed, dirs);
            Assert.Equal(3f, stats.GetAverageSpeed(WindDirection8.North), 5);
        }

        [Fact]
        public void From_ThrowsWhenLengthMismatch()
        {
            Assert.Throws<ArgumentException>(() =>
                WindDirectionStats.From(new float[] { 1f }, new[] { WindDirection8.North, WindDirection8.South }));
        }

        [Fact]
        public void Average_AveragesCorrectly()
        {
            // WindDirectionStats.Average is internal; verify averaging indirectly via MonthWeatherStatsData.Average
            var spd = new float[8];
            var prob1 = new float[8]; prob1[(int)WindDirection8.North] = 1f;
            var prob2 = new float[8]; prob2[(int)WindDirection8.North] = 0f;
            var mma = new MinMaxAvg(0f, 0f, 0f);
            var mmas = new MinMaxAvgStats(mma, mma, mma);
            var m1 = new MonthWeatherStatsData(mma, mmas, mmas, new WindDirectionStats(prob1, spd));
            var m2 = new MonthWeatherStatsData(mma, mmas, mmas, new WindDirectionStats(prob2, spd));
            var avg = MonthWeatherStatsData.Average(new[] { m1, m2 });
            Assert.Equal(0.5f, avg.WindDirection.Probability[(int)WindDirection8.North]);
        }
    }
}
