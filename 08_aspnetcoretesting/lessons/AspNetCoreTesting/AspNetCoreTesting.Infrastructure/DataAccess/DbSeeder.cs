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
            var idGenerator = new IdGenerator();

            if (!dbContext.Professors.Any())
            {
                dbContext.Professors.AddRange(new[]
                {
                    Professor.Create(idGenerator.NewEntityId(), "Jason", "Bourne", "jasonbourne@th-norris.de", "JaBo"),
                    Professor.Create(idGenerator.NewEntityId(), "Chuck", "Norris", "chucknorris@th-norris.de", "ChNo"),
                    Professor.Create(idGenerator.NewEntityId(), "Katie", "Bouman", "dr.katie.bouman@th-norris.de", "KaBo"),
                });

                dbContext.SaveChanges();
            }

            if (!dbContext.Students.Any())
            {
                dbContext.Students.AddRange(new[]
                {
                    Student.Create(idGenerator.NewEntityId(), "Hansi", "Hinterseer", "alpenkasperl@th-norris.de", "MaiHaHi"),
                    Student.Create(idGenerator.NewEntityId(), "Florian", "Silberfischel", "silberfischel@th-norris.de", "FlSi"),
                });

                dbContext.SaveChanges();
            }

            var students = dbContext.Students.ToList();
            var professors = dbContext.Professors.Include(p=>p.AssignedCourses).ToList();
        }
    }
}
