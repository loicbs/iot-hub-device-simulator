namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// The options of the device.
    /// </summary>
    public class DeviceOptions
    {
        /// <summary>
        /// The global endpoint of the device provisioning service.
        /// </summary>
        /// <example>global.azure-devices-provisioning.net</example>
        public string DeviceProvisioningServiceGlobalEndpoint { get; set; } = "global.azure-devices-provisioning.net";

        /// <summary>
        /// The scope of the device provisioning service.
        /// </summary>
        /// <example>123456789</example>
        public string DeviceProvisioningServiceScope { get; set; }

        /// <summary>
        /// The path of the certificate to use.
        /// </summary>
        /// <example>C:\\MyUser\\certificate.pfx</example>
        public string DeviceCertificatePath { get; set; }

        /// <summary>
        /// The certificate password.
        /// </summary>
        /// <example>123456789</example>
        public string DeviceCertificatePassword { get; set; }

    }
}
