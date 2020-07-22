using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreGrpc.Api.Services
{
    public class SensorService: SensorReadingService.SensorReadingServiceBase
    {
        public override Task<SensorResponseMessage> AddReading(SensorReadingPackage request, ServerCallContext context)
        {
            // Do Stuff
            var result = new SensorResponseMessage { Success = true, Message = "Got your reading" };

            foreach (var sensorData in request.Readings)
            {
                // Save all readings
            }

            return Task.FromResult(result);
        }
    }
}
