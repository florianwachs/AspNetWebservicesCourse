using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;

namespace SerilogLesson.Repositories
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