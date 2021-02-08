using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceSimulator.ConsoleApp.Models
{
    /// <summary>
    /// The weather information.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// The main weather information.
        /// </summary>
        [JsonPropertyName("main")]
        public MainInformation Main { get; set; }

        /// <summary>
        /// The wind information.
        /// </summary>
        [JsonPropertyName("wind")]
        public WindInformation Wind { get; set; }
    }

    /// <summary>
    /// The main weather information.
    /// </summary>
    public class MainInformation
    {
        /// <summary>
        /// The current temperature in Kelvin.
        /// </summary>
        /// <example>1023</example>
        [JsonPropertyName("temp")]
        public float Temperature { get; set; }

        /// <summary>
        /// The current Athmospheric pressure in hPa.
        /// </summary>
        /// <example>1023</example>
        [JsonPropertyName("pressure")]
        public float Pressure { get; set; }

        /// <summary>
        /// The current humidity in %.
        /// </summary>
        /// <example>100</example>
        [JsonPropertyName("humidity")]
        public float Humidity { get; set; }
    }

    /// <summary>
    /// The wind information.
    /// </summary>
    public class WindInformation
    {
        /// <summary>
        /// The wind speed in meter/sec.
        /// </summary>
        /// <example>1023</example>
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        /// <summary>
        /// The wind direction in degrees.
        /// </summary>
        /// <example>1023</example>
        [JsonPropertyName("deg")]
        public float Direction { get; set; }
    }
}
