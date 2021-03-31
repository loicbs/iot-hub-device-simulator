using DeviceSimulator.ConsoleApp.Helpers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// The device service.
    /// </summary>
    public class DeviceService : IDeviceService, IDisposable
    {
        private bool disposed;
        private readonly ProvisioningDeviceClient provisioningDeviceClient;
        private readonly SecurityProviderX509Certificate security;
        private readonly ProvisioningTransportHandlerAmqp transport;
        private DeviceClient deviceClient;
        private static string deviceLocation = "Lyon";
        private static int telemetryInterval = 5000;

        private readonly IWeatherService weatherService;
        private readonly DeviceOptions options;
        private readonly ILogger logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceService"/> class.
        /// </summary>
        /// <param name="weatherService">The weather service.</param>
        /// <param name="options">The device options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="weatherService"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="loggerFactory"/> is a null reference.</exception>
        public DeviceService(IOptions<DeviceOptions> options, IWeatherService weatherService, ILoggerFactory loggerFactory)
        {
            this.weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
            logger = loggerFactory?.CreateLogger(GetType().Name) ?? throw new ArgumentNullException(nameof(loggerFactory));

            // Create the provisioning client.
            var certificate = CertificateHelpers.LoadCertificate(this.options.DeviceCertificatePath, this.options.DeviceCertificatePassword, logger);
            security = new SecurityProviderX509Certificate(certificate);
            transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);
            provisioningDeviceClient = ProvisioningDeviceClient.Create(
                this.options.DeviceProvisioningServiceGlobalEndpoint,
                this.options.DeviceProvisioningServiceScope,
                security,
                transport
            );
        }

        /// <summary>
        /// Runs the device simulator.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var registrationResult = await RegisterDeviceAsync(cancellationToken);

            // Create x509 DeviceClient Authentication.
            logger.LogInformation("Creating X509 DeviceClient authentication.");
            var deviceAuthentication = new DeviceAuthenticationWithX509Certificate(registrationResult.DeviceId, security.GetAuthenticationCertificate());
            deviceClient = DeviceClient.Create(registrationResult.AssignedHub, deviceAuthentication, TransportType.Amqp);

            // Explicitly open DeviceClient to communicate with Azure IoT Hub.
            logger.LogInformation("DeviceClient OpenAsync.");
            await deviceClient.OpenAsync();

            // Setup OnDesiredPropertyChanged Event Handling to receive Desired Properties changes.
            logger.LogInformation("Connecting SetDesiredPropertyUpdateCallbackAsync event handler...");
            await deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChangedAsync, null);
            
            // Setup the default direct methods handler. To manage a particular method, use the SetMethodHandlerAsync method.
            logger.LogInformation("Configuring remote command handler...");
            await deviceClient.SetMethodDefaultHandlerAsync(OnDefaultCommandReceivedAsync, null);

            // Load Device Twin Properties since device is just starting up.
            logger.LogInformation("Loading Device Twin Properties...");
            var twin = await deviceClient.GetTwinAsync();
            await OnDesiredPropertyChangedAsync(twin.Properties.Desired, null);

            // Send telemetries.
            logger.LogInformation("Start reading and sending device telemetry..");
            await SendTelemetriesAsync(cancellationToken);
        }

        /// <summary>
        /// Registers the device with the DPS
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>The <see cref="DeviceRegistrationResult"/>.</returns>
        private async Task<DeviceRegistrationResult> RegisterDeviceAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("ProvisioningClient RegisterAsync . . . ");
            var result = await provisioningDeviceClient.RegisterAsync(cancellationToken);

            if (result.Status == ProvisioningRegistrationStatusType.Assigned)
            {
                logger.LogInformation($"Device Registration Status: {result.Status}");
                logger.LogInformation($"ProvisioningClient AssignedHub: {result.AssignedHub}; DeviceID: {result.DeviceId}");
            }
            else
            {
                logger.LogError($"Device Registration Status: {result.Status}");
                throw new Exception($"DeviceRegistrationResult.Status is NOT 'Assigned'");
            }

            return result;
        }

        /// <summary>
        /// Handles desired properties changes.
        /// </summary>
        /// <param name="desiredProperties">The twin desired properties.</param>
        /// <param name="userContext">The context of the user.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        private async Task OnDesiredPropertyChangedAsync(TwinCollection desiredProperties, object userContext)
        {
            logger.LogInformation("Desired Twin Property Changed:");
            logger.LogInformation($"{desiredProperties.ToJson()}");

            // Read the desired Twin Properties.
            if (desiredProperties.Contains("location"))
            {
                deviceLocation = desiredProperties["location"];
                logger.LogInformation($"Setting the desired temperlocationature to: {deviceLocation}");
            }

            if (desiredProperties.Contains("telemetryInterval"))
            {
                telemetryInterval = desiredProperties["telemetryInterval"];
                logger.LogInformation($"Setting the telemetry interval to: {telemetryInterval}");
            }

            // Report Twin properties.
            var reportedProperties = new TwinCollection();
            reportedProperties["location"] = deviceLocation;
            reportedProperties["telemetryInterval"] = telemetryInterval;
            await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
            logger.LogInformation($"Reported Twin Properties: {reportedProperties.ToJson()}");
        }
        /// <summary>
        /// Handles all methods that are not explicitly handled.
        /// </summary>
        /// <param name="methodRequest">The data structure that represents a method request.</param>
        /// <param name="userContext">The context of the user.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        
        private Task<MethodResponse> OnDefaultCommandReceivedAsync(MethodRequest methodRequest, object userContext)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Command received {methodRequest.Name}");
            Console.ResetColor();

            string result = $"{{\"result\":\"Executed direct method: {methodRequest.Name}\"}}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        /// <summary>
        /// Sends telemetry to the iot hub.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        private async Task SendTelemetriesAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var weather = await weatherService.GetCurrentWeatherAsync(deviceLocation, cancellationToken);

                // Create JSON message.
                var json = JsonSerializer.Serialize(weather);
                var message = new Message(Encoding.ASCII.GetBytes(json));

                logger.LogInformation($"Message data: {json}");

                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                logger.LogInformation("Message sent");

                await Task.Delay(telemetryInterval);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting 
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
            => Dispose(true);

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                deviceClient.Dispose();
                transport.Dispose();
                security.Dispose();
            }
            disposed = true;
        }
    }
}
