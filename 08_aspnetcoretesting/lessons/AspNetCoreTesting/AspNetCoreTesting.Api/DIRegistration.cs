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

namespace AspNetCoreTesting.Api
{
    public static class DIRegistration
    {
        public static void AddUniversityServices(this IServiceCollection services)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<UniversityDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
