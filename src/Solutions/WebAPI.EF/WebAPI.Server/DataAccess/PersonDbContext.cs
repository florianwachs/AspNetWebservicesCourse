using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPI.Server.Models;

namespace WebAPI.Server.DataAccess
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext() : base("PersonDbContext")
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}