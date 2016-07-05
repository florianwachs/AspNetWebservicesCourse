using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.IoC.Autofac.Infrastructure
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}
