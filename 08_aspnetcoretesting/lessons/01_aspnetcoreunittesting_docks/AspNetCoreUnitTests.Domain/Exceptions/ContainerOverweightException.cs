using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Exceptions
{
    public class ContainerOverweightException : Exception
    {
        public string ContainerId { get; }

        public ContainerOverweightException(string containerId)
        {
            ContainerId = containerId;
        }
       
    }
}
