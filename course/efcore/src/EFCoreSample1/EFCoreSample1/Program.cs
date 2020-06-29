using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreSample1.DataAccess;
using EFCoreSample1.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFCoreSample1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await MigrateAndSeedDb(host);
            await host.RunAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task MigrateAndSeedDb(IHost host)
        {
            // Um auf das DI-System zuzugreifen muss ein neuer Scope erstellt werden,
            // in dem die erzeugten Objekte "leben"
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
            
            // Mittels Migrate werden alle ausstehenden Db-Migrationen angewendet.
            // Vorsicht wenn mehrere Instanzen versuchen das Upgrade der DB auszuführen.
            // In Produktivsystemen führt man das DB-Upgrade meist getrennt vom Applikationsstart aus.
            await dbContext.Database.MigrateAsync();

            // Häufig werden beim initialen Anlegen der DB einige Stammdaten benötigt.
            // Der Prozess des Befüllens wird oft als Seeding bezeichnet.
            await SeedDb(dbContext);
        }

        private static async Task SeedDb(BookDbContext dbContext)
        {
            // Zuerst prüfen wir ob schon etwas in der DB liegt
            if (dbContext.Books.Any() || dbContext.Authors.Any())
            {
                return;
            }
            
            // Falls nicht, legen wir ein paar Daten an
            var authors = new List<Author>()
            {
                new Author(){ Id = 1, Age = 40, FirstName = "Alice", LastName = "Walker"}, // The Color Purple, Possessing the Secret of Joy, The Temple of My Familiar
                new Author(){ Id = 2, Age = 30, FirstName = "Barbara", LastName = "Oakley"}, // A Mind for Numbers: How to Excel at Math and Science (Even If You Flunked Algebra)
                new Author(){ Id = 3, Age = 20, FirstName = "Chuck", LastName = "Norris"} // The Perfect Roundhouse Kick
            };

            await dbContext.Authors.AddRangeAsync(authors);
            await dbContext.SaveChangesAsync();

        }
    }
}
