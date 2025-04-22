using EFCoreSample1.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample1.DataAccess;

public class DbSeeder
{
    public async Task Seed(IServiceProvider provider)
    {
        // Um auf das DI-System zuzugreifen muss ein neuer Scope erstellt werden,
        // in dem die erzeugten Objekte "leben"
        using var scope = provider.CreateScope();
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

        var alice = new Author
        {
            Id = 1,
            Age = 40,
            FirstName = "Alice",
            LastName = "Walker"
        };
        var barbara = new Author
        {
            Id = 2,
            Age = 30,
            FirstName = "Barbara",
            LastName = "Oakley"
        };

        var chuck = new Author { Id = 3, Age = 20, FirstName = "Chuck", LastName = "Norris" };

        var authors = new List<Author>
            {
                alice,
                barbara,
                chuck,
            };

        await dbContext.Authors.AddRangeAsync(authors);
        await dbContext.SaveChangesAsync();

        var books = new List<Book>
            {
                new Book
                {
                    Isbn = "101", ReleaseDate = DateTime.Today.AddYears(-10), Title = "The Color Purple",
                    Authors = new[] {alice}
                },
                new Book
                {
                    Isbn = "102", ReleaseDate = DateTime.Today.AddYears(-8), Title = "Possessing the Secret of Joy",
                    Authors = new[] { alice}
                },
                new Book
                {
                    Isbn = "103", ReleaseDate = DateTime.Today.AddYears(-5), Title = "The Temple of My Familiar",
                    Authors = new[] {alice}
                },
                new Book
                {
                    Isbn = "201", ReleaseDate = DateTime.Today.AddYears(-2),
                    Title = "A Mind for Numbers: How to Excel at Math and Science (Even If You Flunked Algebra)",
                    Authors = new[] { barbara}
                },
                new Book
                {
                    Isbn = "301", ReleaseDate = DateTime.Today.AddYears(-2), Title = "The Perfect Roundhouse Kick",
                    Authors = new[] { chuck}
                },
            };

        await dbContext.Books.AddRangeAsync(books);
        await dbContext.SaveChangesAsync();
    }
}
