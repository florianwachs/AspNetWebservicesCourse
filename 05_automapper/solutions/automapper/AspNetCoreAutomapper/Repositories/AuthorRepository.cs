using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyCollection<Author>> GetAllWithJokes()
        {
            var result = await _dbContext.Authors.Include(a => a.Jokes).ToListAsync();
            return result;
        }

        public async Task<Author> Update(Author author)
        {
            var updated = _dbContext.Authors.Update(author);
            await _dbContext.SaveChangesAsync();
            return updated.Entity;
        }

        public Task<Author> GetById(Guid id)
        {
            return _dbContext.Authors.FindAsync(id);
        }
    }
}