using System;

namespace AspNetCore.Logging.Serilog.Infrastructure
{
    public class DefaultTimeService : ITimeService
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime Today
        {
            get
            {
                return DateTime.Today;
            }
        }
    }
}
