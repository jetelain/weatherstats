using WeatherStats.Databases;
using WeatherStats.Stats;

namespace WeatherStats.Test
{
    public class WeatherStatsDatabaseTest
    {
        // --------------- GetCellIndex ---------------

        [Theory]
        [InlineData(46.25f, 6.25f, 46, 6)]
        [InlineData(46.0f, 6.0f, 46, 6)]
        [InlineData(45.75f, 5.75f, 46, 6)]
        [InlineData(-1.0f, 1.0f, 0, 0)]
        [InlineData(0.0f, 0.0f, 0, 0)]
        [InlineData(51.5f, 359.75f, 52, 360)]
        public void GetCellIndex_ReturnsExpected(float lat, float lon, int expectedLat, int expectedLon)
        {
            var (cellLat, cellLon) = WeatherStatsDatabase.GetCellIndex(lat, lon);
            Assert.Equal(expectedLat, cellLat);
            Assert.Equal(expectedLon, cellLon);
        }

        // --------------- GetCellFileName ---------------

        [Theory]
        [InlineData(46, 6, "ERA5AVG_N46_006.json")]
        [InlineData(0, 0, "ERA5AVG_N00_000.json")]
        [InlineData(-2, 10, "ERA5AVG_S02_010.json")]
        [InlineData(52, 360, "ERA5AVG_N52_360.json")]
        public void GetCellFileName_ReturnsExpected(int lat, int lon, string expected)
        {
            var name = WeatherStatsDatabase.GetCellFileName((lat, lon));
            Assert.Equal(expected, name);
        }

        // --------------- Create factory ---------------

        [Fact]
        public void Create_WithHttpUrl_ReturnsDatabase()
        {
            var db = WeatherStatsDatabase.Create("https://example.com/data/");
            Assert.NotNull(db);
        }

        [Fact]
        public void Create_WithExistingDirectory_ReturnsDatabase()
        {
            var dir = Path.GetTempPath();
            var db = WeatherStatsDatabase.Create(dir);
            Assert.NotNull(db);
        }

        [Fact]
        public void Create_WithMissingDirectory_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                WeatherStatsDatabase.Create(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())));
        }

        // --------------- GetStats with in-memory storage ---------------

        [Fact]
        public async Task GetStats_NormalizesNegativeLongitude()
        {
            var point = MakePoint(46.25f, 6.25f);
            var key = WeatherStatsDatabase.GetCellFileName(WeatherStatsDatabase.GetCellIndex(46.25f, 6.25f));
            var storage = new FakeStorage(new Dictionary<string, List<YearWeatherStatsPoint>>
            {
                [key] = new List<YearWeatherStatsPoint> { point }
            });
            var db = new WeatherStatsDatabase(storage);

            // longitude -353.75 == 6.25 after +360
            var result = await db.GetStats(46.25, -353.75);
            Assert.NotNull(result);
            Assert.Equal(46.25f, result!.Latitude);
        }

        [Fact]
        public async Task GetStats_ReturnsNull_WhenNotFound()
        {
            var storage = new FakeStorage(new Dictionary<string, List<YearWeatherStatsPoint>>());
            var db = new WeatherStatsDatabase(storage);

            var result = await db.GetStats(0, 0);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStats_SnapsToNearestQuarterDegree()
        {
            // 46.26 should snap to 46.25
            var point = MakePoint(46.25f, 6.25f);
            var key = WeatherStatsDatabase.GetCellFileName(WeatherStatsDatabase.GetCellIndex(46.25f, 6.25f));
            var storage = new FakeStorage(new Dictionary<string, List<YearWeatherStatsPoint>>
            {
                [key] = new List<YearWeatherStatsPoint> { point }
            });
            var db = new WeatherStatsDatabase(storage);

            var result = await db.GetStats(46.26, 6.26);
            Assert.NotNull(result);
        }

        // --------------- helpers ---------------

        private static YearWeatherStatsPoint MakePoint(float lat, float lon)
        {
            var wind = new WindDirectionStats(new float[8], new float[8]);
            var mma = new MinMaxAvg(0, 0, 0);
            var mmas = new MinMaxAvgStats(mma, mma, mma);
            var month = new MonthWeatherStatsData(mma, mmas, mmas, wind);
            return new YearWeatherStatsPoint(lat, lon, Enumerable.Repeat(month, 12).ToArray());
        }

        private sealed class FakeStorage : IWeatherStatsStorage
        {
            private readonly Dictionary<string, List<YearWeatherStatsPoint>> _data;

            public FakeStorage(Dictionary<string, List<YearWeatherStatsPoint>> data)
            {
                _data = data;
            }

            public Task<List<YearWeatherStatsPoint>> Load(string path)
            {
                _data.TryGetValue(path, out var list);
                return Task.FromResult(list ?? new List<YearWeatherStatsPoint>());
            }
        }
    }
}
