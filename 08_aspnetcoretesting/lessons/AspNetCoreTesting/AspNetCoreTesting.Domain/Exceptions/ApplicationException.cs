using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException() : base()
        {
        }

        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
