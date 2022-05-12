using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisService.Repositories;

public class EfJokeRepository : IJokeRepository
{
    private static readonly Random _rnd = new Random();
    private readonly JokeDbContext _dbContext;

    public EfJokeRepository(JokeDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Joke> Add(Joke joke)
    {
        EnsureId(joke);
        var result = await _dbContext.Jokes.AddAsync(joke);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Delete(string id)
    {
        Joke jokeToDelete = await GetJokeById(id);
        if (jokeToDelete == null)
        {
            return;
        }

        _dbContext.Jokes.Remove(jokeToDelete);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<Joke[]> GetAll()
    {
        return await _dbContext.Jokes.Include(j => j.Categories).ToArrayAsync();
    }

    public async Task<Joke?> GetJokeById(string id)
    {
        return await _dbContext.Jokes.Include(j => j.Categories).Where(j => j.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Joke> GetRandomJokeAsync()
    {
        List<string> allIds = await _dbContext.Jokes.Select(j => j.Id).ToListAsync();
        string randomId = allIds[_rnd.Next(0, allIds.Count)];
        Joke joke = await GetJokeById(randomId);
        return joke;
    }

    public async Task<Joke> Update(Joke joke)
    {
        var updated = _dbContext.Jokes.Update(joke);
        await _dbContext.SaveChangesAsync();
        return updated.Entity;
    }

    private void EnsureId(Joke joke)
    {
        if (string.IsNullOrWhiteSpace(joke.Id))
        {
            joke.Id = Guid.NewGuid().ToString();
        }
    }
}
