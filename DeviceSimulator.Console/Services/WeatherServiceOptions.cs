namespace DeviceSimulator.ConsoleApp.Services
{
    /// <summary>
    /// The options of the weather service.
    /// </summary>
    public class WeatherServiceOptions
    {
        /// <summary>
        /// The Api key for the weather provider api.
        /// </summary>
        /// <example>123456789</example>
        public string ApiUrl { get; set; } = "http://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";

        /// <summary>
        /// The Api key for the weather provider api.
        /// </summary>
        /// <example>123456789</example>
        public string ApiKey { get; set; }
    }
}
