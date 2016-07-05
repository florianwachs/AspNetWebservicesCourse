using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.IoC.Autofac.Infrastructure
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