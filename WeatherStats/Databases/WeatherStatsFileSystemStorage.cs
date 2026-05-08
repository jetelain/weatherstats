using System.Text.Json;
using WeatherStats.Stats;

namespace WeatherStats.Databases
{
    /// <summary>
    /// <see cref="IWeatherStatsStorage"/> implementation that reads data files from the local file system.
    /// </summary>
    public sealed class WeatherStatsFileSystemStorage : IWeatherStatsStorage
    {
        private readonly string basePath;

        /// <summary>
        /// Initializes a new instance pointing at the given directory.
        /// </summary>
        /// <param name="basePath">Absolute or relative path to the directory that contains the data files.</param>
        public WeatherStatsFileSystemStorage(string basePath)
        {
            this.basePath = basePath;
        }

        /// <inheritdoc/>
        public async Task<List<YearWeatherStatsPoint>> Load(string path)
        {
            var file = Path.Combine(basePath, path);
            if (File.Exists(file))
            {
                using (var input = File.OpenRead(file))
                {
                    return (await JsonSerializer.DeserializeAsync<List<YearWeatherStatsPoint>>(input).ConfigureAwait(false))!;
                }
            }
            return new List<YearWeatherStatsPoint>();
        }
    }
}
