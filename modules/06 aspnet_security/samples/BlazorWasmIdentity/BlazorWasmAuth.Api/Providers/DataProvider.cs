using BlazorWasmAuth.Shared.Domain;
using Bogus;

namespace BlazorWasamAuth.Api.Providers;

public class DataProvider
{
    private readonly List<Author> _authors;
    public DataProvider()
    {
        // 👇Ein "Seed" setzen um immer die gleichen Daten zu generieren.
        Randomizer.Seed = new Random(8675309);
        int authorId = 1;
        
        // 👇 das NuGet-Paket "Bogus" ist nützlich zum Generieren von realistischen Testdaten
        var authorGenerator = new Faker<Author>()
            .RuleFor(a => a.Id, f => authorId++)
            .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
            .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
            .RuleFor(u => u.EMail, (f, u) => f.Internet.Email(u.FirstName, u.LastName));

        _authors = authorGenerator.Generate(40);
    }
    
    public async Task<List<Author>> GetAuthors()
    {
        return _authors.ToList();
    }

    public async Task<Author?> GetAuthorById(int id) => _authors.FirstOrDefault(i => i.Id == id);
}