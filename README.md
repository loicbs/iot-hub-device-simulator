# IoT Hub device simulator
A device simulator to demonstrate connection to the IoT Hub with DPS and telemetry sending

## Getting Started

### Prerequisites

#### DotNet Core
This project use DotNet core 3.1, you can download the SDK on [this page](https://dotnet.microsoft.com/download).

#### OpenWeather API key
The device simulator uses the [OpenWeather API](https://openweathermap.org/api) to send real weather data. Therefore, you must [create a free account](https://home.openweathermap.org/users/sign_up) on this platform and get an API key.

> If you don't want to use this API, you can replace the [WeatherService](../main/DeviceSimulator.Console/Services/WeatherService.cs) implementation.

#### Azure infrastructure
The device simulator requires an Azure IoT hub and an Azure Device Provisioning Service to run.
You can follow the steps 1 to 5 of the [Automatically provision IoT devices securely and at scale with the Device Provisioning Service](https://docs.microsoft.com/en-us/learn/modules/securely-provision-iot-devices-at-scale-with-device-provisioning-service/) Microsoft Learn module to set up the required Azure services and generate the X.509 certificates that will be used the DPS and the simulator.

### Installation
1. Clone the repository
   ```sh
   git clone https://github.com/loicbs/iot-hub-device-simulator.git
   ```
2. Run the console
   ```sh
   cd DeviceSimulator.Console
   dotnet run --apiKey "<your OpenWeather API key>" --dpsScope "<your DPS scope>" --certificatePath "<path to the device certificate>" --certificatePassword "<device certificate password>"
   ```

> If you followed the MS learn module, the certificate password is '1234'


### Twin support
The device simulator support two twins:
* **location**: Defines the location of the device to obtain local weather information.
* **telemetryInterval**: Defines the time interval between the sending of two telemetries.
