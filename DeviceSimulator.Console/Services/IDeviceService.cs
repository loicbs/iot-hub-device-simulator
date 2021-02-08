using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// Defines the device service.
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Runs the device simulator.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}
