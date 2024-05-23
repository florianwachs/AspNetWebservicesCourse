using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AspNetGrpc.Services;

public class SensorService : SensorReadingService.SensorReadingServiceBase
{
    private static ConcurrentQueue<SensorReadingMessage> _sensorReadings = new ConcurrentQueue<SensorReadingMessage>();
    private static SensorReadingMessage? _latestSensorReading = null;
    public SensorService(ILogger<SensorService> logger)
    {
        Logger = logger;
    }

    public ILogger<SensorService> Logger { get; }

    public override Task<SensorResponseMessage> AddReading(SensorReadingPackage request, ServerCallContext context)
    {
        // Do Stuff
        var result = new SensorResponseMessage { Success = true, Message = "Got your reading" };

        foreach (var sensorData in request.Readings)
        {
            Logger.LogInformation("{SensorId} {Humidity}", sensorData.SensorId, sensorData.Humidity);
            _sensorReadings.Enqueue(sensorData);
            _latestSensorReading = sensorData;
        }

        return Task.FromResult(result);
    }

    public override async Task GetUpdates(Empty request, IServerStreamWriter<SensorReadingMessage> responseStream, ServerCallContext context)
    {

        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (_sensorReadings.TryDequeue(out var reading))
            {
                await responseStream.WriteAsync(reading);
                Logger.LogInformation("Sending sensor reading response");
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

        }
    }

    public override Task<LatestSensoreReadingResponseMessage> GetLatestReading(Empty request, ServerCallContext context)
    {
        LatestSensoreReadingResponseMessage result = new()
        {
            HasValue = _latestSensorReading != null,
            Reading = _latestSensorReading
        };
        
        return Task.FromResult(result);
    }
}