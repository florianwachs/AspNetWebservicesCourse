using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISample.Api.Services
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }
}
