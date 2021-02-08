using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace DeviceSimulator.ConsoleApp.Helpers
{
    /// <summary>
    /// The helper method for X.509 certificates.
    /// </summary>
    public static class CertificateHelpers
    {

        /// <summary>
        /// Loads a X.509 certificate.
        /// </summary>
        /// <param name="path">The certificate path to load.</param>
        /// <param name="password">The certificate password to load.</param>
        /// <returns>The loaded certificate.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> did not contain any certificate with a private key.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null or empty.</exception>
        public static X509Certificate2 LoadCertificate(string path, string password, ILogger logger)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(path, password, X509KeyStorageFlags.UserKeySet);

            X509Certificate2 certificate = null;

            foreach (X509Certificate2 element in certificateCollection)
            {
                logger.LogInformation($"Found certificate: {element?.Thumbprint} {element?.Subject}; PrivateKey: {element?.HasPrivateKey}");
                if (certificate == null && element.HasPrivateKey)
                {
                    certificate = element;
                }
                else
                {
                    element.Dispose();
                }
            }

            if (certificate == null)
            {
                throw new FileNotFoundException($"{path} did not contain any certificate with a private key.");
            }

            logger.LogInformation($"Using certificate {certificate.Thumbprint} {certificate.Subject}");
            return certificate;
        }
    }
}
