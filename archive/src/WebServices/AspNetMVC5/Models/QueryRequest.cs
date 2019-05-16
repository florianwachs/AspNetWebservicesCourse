using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5.Models
{
    public class QueryRequest
    {
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }
}