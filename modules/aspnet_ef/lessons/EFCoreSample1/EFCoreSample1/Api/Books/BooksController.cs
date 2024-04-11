using EFCoreSample1.DataAccess;
using EFCoreSample1.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample1.Api.Books
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookDbContext _dbContext;

        public BooksController(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<Book>> GetBooks()
        {
            return await _dbContext.Books.ToListAsync();
        }
    }
}
