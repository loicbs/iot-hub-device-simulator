using DeviceSimulator.ConsoleApp.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// Defines the weather service.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Gets the current weather of a given city.
        /// </summary>
        /// <param name="cityName">The name of the city to be searched.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>The weather information.</returns>
        Task<Weather> GetCurrentWeatherAsync(string cityName, CancellationToken cancellationToken = default);
    }
}
