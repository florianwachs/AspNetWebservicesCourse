using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreSwagger.DataAccess;
using AspNetCoreSwagger.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSwagger.Repositories
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