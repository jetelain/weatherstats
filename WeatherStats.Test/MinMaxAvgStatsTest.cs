using Pmad.WeatherStats.Stats;

namespace Pmad.WeatherStats.Test
{
    public class MinMaxAvgStatsTest
    {
        [Fact]
        public void From_FloatArray_SlicesCorrectly()
        {
            // 48 values, slice=24 -> 2 days
            var values = Enumerable.Range(1, 48).Select(i => (float)i).ToArray();
            var result = MinMaxAvgStats.From(values, 24);

            Assert.Equal(1f, result.Min.Min);
            Assert.Equal(48f, result.Max.Max);
        }

        [Fact]
        public void Average_AveragesMultipleInstances()
        {
            // Use MinMaxAvg.Average (public) to verify averaging logic on the public type
            var a = new MinMaxAvg(1f, 2f, 3f);
            var b = new MinMaxAvg(3f, 4f, 5f);
            var avg = MinMaxAvg.Average(new[] { a, b });
            Assert.Equal(2f, avg.Min);
            Assert.Equal(3f, avg.Avg);
            Assert.Equal(4f, avg.Max);
        }
    }
}
