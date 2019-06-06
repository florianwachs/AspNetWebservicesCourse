using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuckNorrisService.Repositories
{
    public class EFJokeRepository : IJokeRepository
    {
        private static readonly Random rnd = new Random();
        private readonly JokeDbContext _dbContext;
        private readonly DbConnectionString _dbConnectionString;

        public EFJokeRepository(JokeDbContext dbContext, DbConnectionString dbConnectionString)
        {
            _dbContext = dbContext;
            _dbConnectionString = dbConnectionString;
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

        public async Task<IReadOnlyCollection<JokeOnly>> GetJokes()
        {
            using (var connection = new SqlConnection(_dbConnectionString.ConnectionString))
            {
                var jokes = (await connection.QueryAsync<JokeOnly>("Select * FROM jokes")).ToList();
                return jokes;
            }
        }
    }
}
