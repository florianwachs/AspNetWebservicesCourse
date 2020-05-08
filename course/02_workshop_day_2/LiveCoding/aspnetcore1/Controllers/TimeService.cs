using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore1.Controllers
{
    public interface ITimeService
    {
        DateTime GetCurrentDate();
        DateTime GetCurrentDateAndTime();
    }

    public class TimeService : ITimeService
    {

        public DateTime GetCurrentDate() => DateTime.Today;

        public DateTime GetCurrentDateAndTime() => DateTime.Now;
    }
}
