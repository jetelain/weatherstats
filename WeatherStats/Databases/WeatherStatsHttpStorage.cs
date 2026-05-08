using System.Text.Json;
using WeatherStats.Stats;

namespace WeatherStats.Databases
{
    /// <summary>
    /// <see cref="IWeatherStatsStorage"/> implementation that fetches data files over HTTP/HTTPS.
    /// </summary>
    public sealed class WeatherStatsHttpStorage : IWeatherStatsStorage
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance using the provided <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">Pre-configured <see cref="HttpClient"/>. Its <c>BaseAddress</c> must be set.</param>
        public WeatherStatsHttpStorage(HttpClient client)
        {
            this.client = client;
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0");
        }

        /// <summary>
        /// Initializes a new instance that creates its own <see cref="HttpClient"/> with the given base address.
        /// </summary>
        /// <param name="baseAddress">Base URI of the data storage (e.g. <c>https://weatherdata.pmad.net/ERA5AVG/</c>).</param>
        public WeatherStatsHttpStorage(Uri baseAddress)
            : this(new HttpClient() { BaseAddress = baseAddress })
        {

        }

        /// <inheritdoc/>
        public async Task<List<YearWeatherStatsPoint>> Load(string path)
        {
            try
            {
                using (var input = await client.GetStreamAsync(path).ConfigureAwait(false))
                {
                    return (await JsonSerializer.DeserializeAsync<List<YearWeatherStatsPoint>>(input).ConfigureAwait(false))!;
                }
            }
            catch(HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<YearWeatherStatsPoint>();
            }
        }
    }
}
