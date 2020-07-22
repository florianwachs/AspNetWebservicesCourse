using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreGrpc.Api.Services
{
    public class SensorService: SensorReadingService.SensorReadingServiceBase
    {
        private static ConcurrentQueue<SensorReadingMessage> _sensorReadings = new ConcurrentQueue<SensorReadingMessage>();

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
            }

            return Task.FromResult(result);
        }

        public override async Task GetUpdates(Empty request, IServerStreamWriter<SensorReadingMessage> responseStream, ServerCallContext context)
        {
            
            while (!context.CancellationToken.IsCancellationRequested)
            {

                if(_sensorReadings.TryDequeue(out var reading))
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
    }
}
