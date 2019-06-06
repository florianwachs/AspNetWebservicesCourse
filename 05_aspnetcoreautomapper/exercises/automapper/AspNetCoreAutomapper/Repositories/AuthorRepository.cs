using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAutomapper.DataAccess;
using AspNetCoreAutomapper.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreAutomapper.Repositories
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