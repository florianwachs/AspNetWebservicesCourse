using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.ApplicationServices;
using AspNetCoreTesting.Domain.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTesting.Infrastructure.DataAccess
{
    public static class DbSeeder
    {
        public static void SeedDb(UniversityDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            var id = new IdGenerator();

            if (!dbContext.Professors.Any())
            {
                dbContext.Professors.AddRange(new[]
                {
                    Professor.Create(id.NewEntityId(), "Jason", "Bourne", "jasonbourne@th-norris.de", "JaBo"),
                    Professor.Create(id.NewEntityId(), "Chuck", "Norris", "chucknorris@th-norris.de", "ChNo"),
                    Professor.Create(id.NewEntityId(), "Katie", "Bouman", "dr.katie.bouman@th-norris.de", "KaBo"),
                });

                dbContext.SaveChanges();
            }

            if (!dbContext.Students.Any())
            {
                dbContext.Students.AddRange(new[]
                {
                    Student.Create(id.NewEntityId(), "Hansi", "Hinterseer", "alpenkasperl@th-norris.de", "MaiHaHi"),
                    Student.Create(id.NewEntityId(), "Florian", "Silberfischel", "silberfischel@th-norris.de", "FlSi"),
                    Student.Create(id.NewEntityId(), "Seven", "Of Nine", "resistenceisfutile@th-norris.de", "SeNi"),
                });

                dbContext.SaveChanges();
            }

            if (!dbContext.Courses.Any())
            {
                dbContext.Courses.AddRange(new[]
                {
                    Course.Create(id.NewEntityId(), "Physics_1", "How to take a picture of a black hole", "How to take a picture of a black hole"),
                    Course.Create(id.NewEntityId(), "SelfDefense_1", "Roundhouse Kick", "How to do the perfect Roundhouse Kick."),
                    Course.Create(id.NewEntityId(), "Memory_1", "Memory lost, what now", "Guidelines to cope with memory loss."),
                });
            }
        }
    }
}
