using System.Linq;
using AspNetCoreTesting.Domain.ApplicationServices;
using AspNetCoreTesting.Domain.Domain;

namespace AspNetCoreTesting.Infrastructure.DataAccess
{
    public static class DbSeeder
    {
        private static readonly IdGenerator _id = new IdGenerator();

        public static void SeedDb(UniversityDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            SeedProfessors(dbContext);
            SeedStudents(dbContext);
            SeedCourses(dbContext);
        }

        private static void SeedStudents(UniversityDbContext dbContext)
        {
            if (!dbContext.Students.Any())
            {
                dbContext.Students.AddRange(new[]
                {
                    Student.Create(_id.NewEntityId(), "Hansi", "Hinterseer", "alpenkasperl@th-norris.de", "MaiHaHi"),
                    Student.Create(_id.NewEntityId(), "Florian", "Silberfischel", "silberfischel@th-norris.de", "FlSi"),
                    Student.Create(_id.NewEntityId(), "Seven", "Of Nine", "resistenceisfutile@th-norris.de", "SeNi"),
                });

                dbContext.SaveChanges();
            }
        }

        private static void SeedCourses(UniversityDbContext dbContext)
        {
            if (!dbContext.Courses.Any())
            {
                dbContext.Courses.AddRange(new[]
                {
                    Course.Create(_id.NewEntityId(), "Physics_1", "How to take a picture of a black hole", "How to take a picture of a black hole"),
                    Course.Create(_id.NewEntityId(), "SelfDefense_1", "Roundhouse Kick", "How to do the perfect Roundhouse Kick."),
                    Course.Create(_id.NewEntityId(), "Memory_1", "Memory lost, what now", "Guidelines to cope with memory loss."),
                });

                dbContext.SaveChanges();
            }
        }

        private static void SeedProfessors(UniversityDbContext dbContext)
        {
            if (!dbContext.Professors.Any())
            {
                dbContext.Professors.AddRange(new[]
                {
                    Professor.Create(_id.NewEntityId(), "Jason", "Bourne", "jasonbourne@th-norris.de", "JaBo"),
                    Professor.Create(_id.NewEntityId(), "Chuck", "Norris", "chucknorris@th-norris.de", "ChNo"),
                    Professor.Create(_id.NewEntityId(), "Katie", "Bouman", "dr.katie.bouman@th-norris.de", "KaBo"),
                });

                dbContext.SaveChanges();
            }
        }
    }
}
