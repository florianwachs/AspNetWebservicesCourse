using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using AspNetGrpc.Services;

var channel = GrpcChannel.ForAddress("https://localhost:7070");
var client = new SensorReadingService.SensorReadingServiceClient(channel);

while (true)
{

    var result = await client.AddReadingAsync(GetSensorReadingPackage());
    await Task.Delay(TimeSpan.FromSeconds(5));
}


static SensorReadingPackage GetSensorReadingPackage()
{
    var package = new SensorReadingPackage();

    for (int i = 0; i < Random.Shared.Next(4, 20); i++)
    {
        var sample = new SensorReadingMessage
        {
            Humidity = Random.Shared.Next(10, 100),
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
            SensorId = Random.Shared.Next(1, 5),
            AtmosphericPressure = Random.Shared.Next(200, 4000),
            TemperatureInC = Random.Shared.Next(-30, 100),
            SensorStatus = SensorStatus.Ok,
        };

        package.Readings.Add(sample);
        Console.WriteLine("Package sent");
    }

    return package;
}