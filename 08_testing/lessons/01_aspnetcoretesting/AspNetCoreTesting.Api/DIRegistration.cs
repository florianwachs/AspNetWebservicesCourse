using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using AutoMapper;
using Microsoft.Data.Sqlite;
using AspNetCoreTesting.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using AspNetCoreTesting.Domain.Domain;
using AspNetCoreTesting.Api.StudentManagement;

namespace AspNetCoreTesting.Api
{
    public static class DIRegistration
    {
        public static void AddUniversityServices(this IServiceCollection services)
        {
            // Manuelles erzeugen der SqliteConection damit sie hier geöffnet werden kann
            // Sonst schließt der erste DBContext der Disposed wird die Connection und die
            // Db geht offline
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<UniversityDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

            services.AddAutoMapper(typeof(Student), typeof(StudentsController));
        }
    }
}
