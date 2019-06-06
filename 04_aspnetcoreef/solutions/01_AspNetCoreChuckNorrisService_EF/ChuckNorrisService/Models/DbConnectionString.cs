using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChuckNorrisService.Models
{
    public class DbConnectionString
    {
        public string ConnectionString { get; }
        public DbConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            ConnectionString = connectionString;
        }
    }
}
