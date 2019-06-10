using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreUnitTests.Domain.Exceptions
{
    public class ItemAlreadyInContainerException : Exception
    {

        public string ContainerItemId { get; }

        private ItemAlreadyInContainerException() : base()
        {
        }

        public ItemAlreadyInContainerException(string containerItemId, string message = null, Exception innerException = null) : base(message, innerException)
        {
            ContainerItemId = containerItemId;
        }
    }
}
