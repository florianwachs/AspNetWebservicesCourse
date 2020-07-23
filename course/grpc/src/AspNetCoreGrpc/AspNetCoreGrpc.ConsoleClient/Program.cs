using AspNetCoreGrpc.Api.Services;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace AspNetCoreGrpc.ConsoleClient
{
    class Program
    {
        private static readonly Random random = new Random();
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new SensorReadingService.SensorReadingServiceClient(channel);
            while (true)
            {

                var result = await client.AddReadingAsync(GetSensorReadingPackage());
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private static SensorReadingPackage GetSensorReadingPackage()
        {
            var package = new SensorReadingPackage();

            for (int i = 0; i < random.Next(4, 20); i++)
            {
                var sample = new SensorReadingMessage
                {
                    Humidity = random.Next(10, 100),
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    SensorId = random.Next(1, 5),
                    AtmosphericPressure = random.Next(200, 4000),
                    TemperatureInC = random.Next(-30, 100),
                    SensorStatus = SensorStatus.Ok,
                };

                package.Readings.Add(sample);
            }

            return package;
        }
    }
}
