using Pmad.WeatherStats.Stats;

namespace Pmad.WeatherStats.Databases
{
    /// <summary>
    /// Abstraction over a storage back-end that can load weather statistics data files.
    /// </summary>
    public interface IWeatherStatsStorage
    {
        /// <summary>
        /// Loads the list of <see cref="YearWeatherStatsPoint"/> entries from the file identified by <paramref name="path"/>.
        /// Returns an empty list when the file does not exist.
        /// </summary>
        /// <param name="path">Relative file name (e.g. <c>ERA5AVG_N46_006.json</c>).</param>
        Task<List<YearWeatherStatsPoint>> Load(string path);
    }
}
