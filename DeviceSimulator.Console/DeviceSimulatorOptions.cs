using CommandLine;

namespace DeviceSimulator.ConsoleApp
{
    /// <summary>
    /// The options of the device simulator.
    /// </summary>
    public class DeviceSimulatorOptions
    {
        /// <summary>
        /// The Api key for the weather provider API.
        /// </summary>
        /// <example>123456789</example>
        [Option('a', "apiKey", Required = true, HelpText = "The Api key for the weather provider API.")]
        public string WeatherApiKey { get; set; }

        /// <summary>
        /// The path of the certificate to use.
        /// </summary>
        /// <example>C:\\MyUser\\certificate.pfx</example>
        [Option('c', "certificatePath", Required = true, HelpText = "The path of the certificate to use.")]
        public string DeviceCertificatePath { get; set; }

        /// <summary>
        /// The certificate password.
        /// </summary>
        /// <example>123456789</example>
        [Option('p', "certificatePassword", Required = true, HelpText = "The certificate password.")]
        public string DeviceCertificatePassword { get; set; }

        /// <summary>
        /// The scope of the device provisioning service.
        /// </summary>
        /// <example>123456789</example>
        [Option('s', "dpsScope", Required = true, HelpText = "The scope of the device provisioning service.")]
        public string DeviceProvisioningServiceScope { get; set; }
    }
}
