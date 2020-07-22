using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreGrpc.Api.Services
{
    public class SensorService: SensorReadingService.SensorReadingServiceBase
    {
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
            }

            return Task.FromResult(result);
        }
    }
}
