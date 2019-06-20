using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Frontend.Models
{
    public class ApiConfig
    {
        public string BooksServiceBaseUri { get; set; }
        public string JokesServiceBaseUri { get; set; }
    }
}
