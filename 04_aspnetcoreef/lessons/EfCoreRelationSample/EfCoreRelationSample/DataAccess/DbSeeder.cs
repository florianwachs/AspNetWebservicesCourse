using EfCoreRelationSample.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreRelationSample.DataAccess
{
    public class DbSeeder
    {
        private static readonly IdGenerator _id = new IdGenerator();

        public static void SeedDb(DemoDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            if (dbContext.Lists.Any())
            {
                return;
            }

            SeedLists(dbContext);
        }

        private static void SeedLists(DemoDbContext dbContext)
        {

        }
    }
}
