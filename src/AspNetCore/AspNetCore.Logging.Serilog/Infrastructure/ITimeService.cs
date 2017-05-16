using System;

namespace AspNetCore.Logging.Serilog.Infrastructure
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}
