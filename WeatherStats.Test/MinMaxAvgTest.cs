using Pmad.WeatherStats.Stats;

namespace Pmad.WeatherStats.Test
{
    public class MinMaxAvgTest
    {
        [Fact]
        public void From_ComputesMinAvgMax()
        {
            var result = MinMaxAvg.From(new float[] { 1f, 2f, 3f, 4f, 5f });
            Assert.Equal(1f, result.Min);
            Assert.Equal(3f, result.Avg);
            Assert.Equal(5f, result.Max);
        }

        [Fact]
        public void From_IgnoresNaN()
        {
            var result = MinMaxAvg.From(new float[] { 1f, float.NaN, 3f });
            Assert.Equal(1f, result.Min);
            Assert.Equal(2f, result.Avg);
            Assert.Equal(3f, result.Max);
        }

        [Fact]
        public void From_SingleValue()
        {
            var result = MinMaxAvg.From(new float[] { 7f });
            Assert.Equal(7f, result.Min);
            Assert.Equal(7f, result.Avg);
            Assert.Equal(7f, result.Max);
        }

        [Fact]
        public void From_WithStartAndLength_Slice()
        {
            var values = new float[] { 10f, 1f, 2f, 3f, 10f };
            var result = MinMaxAvg.From(values, 1, 3);
            Assert.Equal(1f, result.Min);
            Assert.Equal(2f, result.Avg);
            Assert.Equal(3f, result.Max);
        }

        [Fact]
        public void Average_AveragesMultipleInstances()
        {
            var a = new MinMaxAvg(1f, 2f, 3f);
            var b = new MinMaxAvg(3f, 4f, 5f);
            var avg = MinMaxAvg.Average(new[] { a, b });
            Assert.Equal(2f, avg.Min);
            Assert.Equal(3f, avg.Avg);
            Assert.Equal(4f, avg.Max);
        }
    }
}
