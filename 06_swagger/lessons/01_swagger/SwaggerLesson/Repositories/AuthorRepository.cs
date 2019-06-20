using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwaggerLesson.DataAccess;
using SwaggerLesson.Models;

namespace SwaggerLesson.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookDbContext _dbContext;

        public AuthorRepository(BookDbContext dbContext)
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