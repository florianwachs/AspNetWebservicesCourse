using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;

namespace SerilogLesson.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext _dbContext;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(BookDbContext dbContext, ILogger<BookRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Book> Add(Book book)
        {
            try
            {
                EnsureId(book);
                var result = await _dbContext.Books.AddAsync(book);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add book with {id}", book?.Id);
                throw;
            }
        }

        private void EnsureId(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Id))
            {
                book.Id = Guid.NewGuid().ToString();
            }
        }

        public async Task Delete(string id)
        {
            var jokeToDelete = await GetById(id);
            if (jokeToDelete == null)
                return;

            _dbContext.Books.Remove(jokeToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Book>> GetAll()
        {
            var jokes = await _dbContext.Books.ToListAsync();
            return jokes;
        }

        public Task<Book> GetById(string id)
        {
            return _dbContext.Books.FindAsync(id);
        }

        public async Task<Book> Update(Book book)
        {
            var updated = _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return updated.Entity;
        }
    }
}
