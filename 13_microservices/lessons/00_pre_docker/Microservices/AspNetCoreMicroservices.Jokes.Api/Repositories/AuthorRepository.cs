using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreMicroservices.Jokes.Api.DataAccess;
using AspNetCoreMicroservices.Jokes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMicroservices.Jokes.Api.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly JokeDbContext _dbContext;

        public AuthorRepository(JokeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<Author>> GetAll()
        {
            var result = await _dbContext.Authors.ToListAsync();
            return result;
        }
    }
}