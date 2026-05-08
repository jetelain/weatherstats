using System.Collections.Concurrent;
using Pmad.WeatherStats.Stats;

namespace Pmad.WeatherStats.Databases
{
    /// <summary>
    /// High-level facade for accessing ERA5 average weather statistics.
    /// Transparently caches loaded cells and resolves geographic coordinates to the nearest grid point.
    /// </summary>
    public class WeatherStatsDatabase
    {
        private readonly ConcurrentDictionary<string, Task<List<YearWeatherStatsPoint>>> cache = new ConcurrentDictionary<string, Task<List<YearWeatherStatsPoint>>>();
        private readonly IWeatherStatsStorage storage;

        /// <summary>
        /// Initializes a new instance backed by the given <see cref="IWeatherStatsStorage"/>.
        /// </summary>
        /// <param name="storage">Storage back-end to use for loading data files.</param>
        public WeatherStatsDatabase(IWeatherStatsStorage storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Creates a <see cref="WeatherStatsDatabase"/> from a URL or a local directory path.
        /// </summary>
        /// <param name="location">
        /// An HTTP/HTTPS URL (e.g. <c>https://weatherdata.pmad.net/ERA5AVG/</c>) or
        /// a path to an existing local directory that contains the data files.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">Thrown when <paramref name="location"/> is not a URL and the directory does not exist.</exception>
        public static WeatherStatsDatabase Create(string location)
        {
            if(location.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || location.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
            { 
                return new WeatherStatsDatabase(new WeatherStatsHttpStorage(new Uri(location))); 
            }
            if (Directory.Exists(location))
            {
                return new WeatherStatsDatabase(new WeatherStatsFileSystemStorage(location));
            }
            throw new DirectoryNotFoundException($"Location '{location}' was not found.");
        }

        /// <summary>
        /// Retrieves the yearly weather statistics for the grid point nearest to the given coordinates.
        /// Returns <see langword="null"/> when no data is available for that point.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees (positive = North, range −90 to 90).</param>
        /// <param name="longitude">Longitude in decimal degrees (positive = East). Negative values are normalised automatically.</param>
        public Task<YearWeatherStatsPoint?> GetStats(double latitude, double longitude)
        {
            if (longitude < 0)
            {
                longitude += 360;
            }
            var pointLatitude = Math.Round(Math.Round(latitude * 4) / 4, 2);
            var pointLongitude = Math.Round(Math.Round(longitude * 4) / 4, 2);
            return GetStatsExact((float)pointLatitude, (float)pointLongitude);
        }

        private async Task<YearWeatherStatsPoint?> GetStatsExact(float pointLatitude, float pointLongitude)
        {
            var name = GetCellFileName(GetCellIndex(pointLatitude, pointLongitude));

            var data = await cache.GetOrAdd(name, storage.Load).ConfigureAwait(false);

            return data.FirstOrDefault(d => d.Latitude == pointLatitude && d.Longitude == pointLongitude);
        }

        /// <summary>
        /// Returns the (latitude, longitude) index of the 2-degree cell that contains the given point.
        /// </summary>
        /// <param name="pointLatitude">Snapped latitude of the grid point.</param>
        /// <param name="pointLongitude">Snapped longitude of the grid point.</param>
        public static (int,int) GetCellIndex(float pointLatitude, float pointLongitude)
        {
            var cellLat = (int)Math.Ceiling(Math.Round(pointLatitude, 2));
            var cellLon = (int)Math.Ceiling(Math.Round(pointLongitude, 2));
            cellLat -= cellLat % 2;
            cellLon -= cellLon % 2;
            return (cellLat, cellLon);
        }

        /// <summary>
        /// Returns the file name for the data cell identified by <paramref name="index"/>
        /// (e.g. <c>ERA5AVG_N46_006.json</c>).
        /// </summary>
        /// <param name="index">Cell index as returned by <see cref="GetCellIndex"/>.</param>
        public static string GetCellFileName((int,int) index)
        {
            (var lat, var lon) = index;
            return FormattableString.Invariant($"ERA5AVG_{(lat < 0 ? "S" : "N")}{Math.Abs(lat):00}_{lon:000}.json");
        }
    }
}
