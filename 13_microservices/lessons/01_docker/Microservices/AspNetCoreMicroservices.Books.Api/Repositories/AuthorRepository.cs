using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreMicroservices.Books.Api.DataAccess;
using AspNetCoreMicroservices.Books.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMicroservices.Books.Api.Repositories
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