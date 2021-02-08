using CommandLine;
using DeviceSimulator.ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp
{
    /// <summary>
    /// Main class for the device simulator.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of the device simulator.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The application return code.</returns>
        static async Task<int> Main(string[] args)
        {
            if (!TryParseAndValidateArguments<DeviceSimulatorOptions>(args, out var options))
                return -1;

            var cancellationTokenSource = new CancellationTokenSource();
            var device = CreateHostBuilder(options).Build().Services.GetRequiredService<IDeviceService>();
            await device.RunAsync(cancellationTokenSource.Token);
            return 0;
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="options">The device simulator options.</param>
        /// <returns>The host builder./></returns>
        private static IHostBuilder CreateHostBuilder(DeviceSimulatorOptions options)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .Filter.ByExcluding(c => c.MessageTemplate.Text.Contains("HTTP", StringComparison.OrdinalIgnoreCase))
              .WriteTo.Console()
              .CreateLogger();

            return Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog(dispose: true);
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((context, services) => ConfigureServices(services, options));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="options">The device simulator options.</param>
        private static void ConfigureServices(IServiceCollection services, DeviceSimulatorOptions options)
        {
            // Services
            services
                .AddHttpClient()
                .AddTransient<IWeatherService, WeatherService>()
                .AddTransient<IDeviceService, DeviceService>();

            // Options
            services.Configure<WeatherServiceOptions>(opt =>
            {
                opt.ApiKey = options.WeatherApiKey;
            })
            .Configure<DeviceOptions>(opt =>
            {
                opt.DeviceCertificatePath = options.DeviceCertificatePath;
                opt.DeviceCertificatePassword = options.DeviceCertificatePassword;
                opt.DeviceProvisioningServiceScope = options.DeviceProvisioningServiceScope;
            });
        }

        /// <summary>
        /// Validates and parses the device simulator options.
        /// </summary>
        /// <param name="options">The device simulator options.</param>
        /// <returns>True if the options are valide; false otherwise./></returns>
        private static bool TryParseAndValidateArguments<T>(string[] args, out T options) where T : class
        {
            var parsingResult = Parser.Default.ParseArguments<T>(args);
            if (parsingResult is Parsed<T> parsingResultSuccess)
            {
                options = parsingResultSuccess.Value;
                var results = new List<ValidationResult>();
                if (Validator.TryValidateObject(options, new ValidationContext(options), results, true))
                    return true;
                else
                {
                    Console.Error.WriteLine("Problems found in command line arguments:");
                    foreach (var result in results)
                    {
                        Console.Error.WriteLine($"- {result.ErrorMessage}");
                    }
                    return false;
                }
            }
            else
            {
                options = default;
                return false;
            }
        }

    }
}
