using AspNetCoreTesting.Api.StudentManagement;
using AspNetCoreTesting.Domain.Domain;
using AspNetCoreTesting.Infrastructure.DataAccess;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
