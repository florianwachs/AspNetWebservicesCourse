using ChuckNorrisService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisService.DataAccess;

public class JokeDbSeeder
{

    public async Task SeedDb(IServiceProvider provider, bool inMemory)
    {
        using var scope = provider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<JokeDbContext>();

        if (!inMemory)
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Jokes.AnyAsync())
        {
            return;
        }

        var repository = new FileSystemJokeRespository();
        var allJokes = await repository.GetAll();

        await dbContext.Jokes.AddRangeAsync(allJokes);
        await dbContext.SaveChangesAsync();
    }

}
