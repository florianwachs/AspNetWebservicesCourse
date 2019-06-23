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

            var list1 = List.Create(_id.NewEntityId(), "Shared With All");
            var list2 = List.Create(_id.NewEntityId(), "Black Hole Pictures");

            var user1 = User.Create(_id.NewEntityId(), "Katie Bouman");
            var user2 = User.Create(_id.NewEntityId(), "Chuck Norris");
            var user3 = User.Create(_id.NewEntityId(), "Jean Claude van Damme");

            list1.ShareWithUser(user1);
            list1.ShareWithUser(user2);
            list1.ShareWithUser(user3);

            list2.ShareWithUser(user1);

            dbContext.Lists.AddRange(new[] { list1, list2 });
            dbContext.Users.AddRange(new[] { user1, user2, user3 });

            dbContext.SaveChanges();
        }


    }
}
