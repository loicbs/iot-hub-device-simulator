using DeviceSimulator.ConsoleApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// Service for <see cref="Weather"/>.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly WeatherServiceOptions options;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The http client factory.</param>
        /// <param name="options">The options of the service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientFactory"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is a null reference.</exception>
        public WeatherService(IHttpClientFactory httpClientFactory, IOptions<WeatherServiceOptions> options)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Gets the current weather of a given city.
        /// </summary>
        /// <param name="cityName">The name of the city to be searched.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>The weather information.</returns>
        public async Task<Weather> GetCurrentWeatherAsync(string cityName, CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient();

            var uri = new Uri(string.Format(CultureInfo.CurrentCulture, options.ApiUrl, cityName, options.ApiKey));
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            // Request the weather API
            var result = await client.SendAsync(request, cancellationToken);
            var content = await result.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Weather>(content, serializerOptions, cancellationToken);
        }
    }
}
