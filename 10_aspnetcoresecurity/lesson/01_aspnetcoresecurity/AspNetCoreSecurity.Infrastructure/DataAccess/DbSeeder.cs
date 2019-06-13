using System.Linq;
using AspNetCoreSecurity.Domain.ApplicationServices;
using AspNetCoreSecurity.Domain.Data;
using AspNetCoreSecurity.Domain.Domain;

namespace AspNetCoreSecurity.Infrastructure.DataAccess
{
    public static class DbSeeder
    {
        private static readonly IdGenerator _id = new IdGenerator();

        public static void SeedDb(UniversityDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            if (dbContext.Students.Any())
            {
                return;
            }

            SeedProfessors(dbContext);
            SeedStudents(dbContext);
            SeedCourses(dbContext);
            AssignKnownProfessors(dbContext);
        }

        private static void SeedStudents(UniversityDbContext dbContext)
        {
            if (!dbContext.Students.Any())
            {
                dbContext.Students.AddRange(KnownUsers.Get().Where(u => u.Type == UserTypes.Student).Select(data => Student.Create(data.Id, data.GivenName, data.FamilyName, data.Email, data.Id)));

                dbContext.SaveChanges();
            }
        }

        private static void SeedCourses(UniversityDbContext dbContext)
        {
            if (!dbContext.Courses.Any())
            {
                dbContext.Courses.AddRange(new[]
                {
                    Course.Create("math_1", "Math_1", "How to take a picture of a black hole", "How to take a picture of a black hole"),
                    Course.Create("physics_1", "Physics_1", "How to take a picture of a black hole", "How to take a picture of a black hole"),
                    Course.Create("selfdefense_1", "SelfDefense_1", "Roundhouse Kick", "How to do the perfect Roundhouse Kick."),
                    Course.Create("memory_1", "Memory_1", "Memory lost, what now", "Guidelines to cope with memory loss."),
                });

                dbContext.SaveChanges();
            }
        }

        private static void SeedProfessors(UniversityDbContext dbContext)
        {
            if (!dbContext.Professors.Any())
            {
                dbContext.Professors.AddRange(KnownUsers.Get().Where(u => u.Type == UserTypes.Professor || u.Type == UserTypes.Principal).Select(data => Professor.Create(data.Id, data.GivenName, data.FamilyName, data.Email, data.Id)));

                dbContext.SaveChanges();
            }
        }

        private static void AssignKnownProfessors(UniversityDbContext dbContext)
        {
            var course = dbContext.Courses.Find("physics_1");
            var professor = dbContext.Professors.Find("katie");

            course.AssignProfessorToCourse(professor);

            dbContext.SaveChanges();
        }
    }
}
