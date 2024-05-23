using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using AspNetGrpc.Services;
using Grpc.Core;

var channel = GrpcChannel.ForAddress("https://localhost:7070");
var client = new SensorReadingService.SensorReadingServiceClient(channel);

var latest = await client.GetLatestReadingAsync(new Empty());

var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
using var streamingCall = client.GetUpdates(new Empty(), cancellationToken: cts.Token);

try
{
    await foreach (var sensorReading in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
    {
        Console.WriteLine($"{sensorReading.SensorId} | {sensorReading.SensorStatus} | {sensorReading.TemperatureInC} C");
    }
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
{               
    Console.WriteLine("Stream cancelled.");
}

latest = await client.GetLatestReadingAsync(new Empty());

Console.ReadKey();