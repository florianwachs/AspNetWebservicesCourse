using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreSwagger.DataAccess;
using AspNetCoreSwagger.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSwagger.Repositories
{
    public class JokeRepository : IJokeRepository
    {
        private static readonly Random rnd = new Random();
        private readonly JokeDbContext _dbContext;

        public JokeRepository(JokeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Joke> Add(Joke joke)
        {
            EnsureId(joke);
            var result = await _dbContext.Jokes.AddAsync(joke);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        private void EnsureId(Joke joke)
        {
            if (string.IsNullOrWhiteSpace(joke.Id))
            {
                joke.Id = Guid.NewGuid().ToString();
            }
        }

        public async Task Delete(string id)
        {
            var jokeToDelete = await GetById(id);
            if (jokeToDelete == null)
                return;

            _dbContext.Jokes.Remove(jokeToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Joke>> GetAll()
        {
            var jokes = await _dbContext.Jokes.ToListAsync();
            return jokes;
        }

        public Task<Joke> GetById(string id)
        {
            return _dbContext.Jokes.FindAsync(id);
        }

        public async Task<Joke> Update(Joke joke)
        {
            var updated = _dbContext.Jokes.Update(joke);
            await _dbContext.SaveChangesAsync();
            return updated.Entity;
        }

        public async Task<Joke> GetRandomJoke()
        {
            var allIds = await _dbContext.Jokes.Select(j => j.Id).ToListAsync();
            var randomId = allIds[rnd.Next(0, allIds.Count)];
            var joke = await GetById(randomId);
            return joke;
        }

    }
}
