using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EFRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EFRepository.Repositories
{
    public class BookRepositoryEF : IBookRepository
    {
        private BookDbContext BookDbContext { get; set; }
        public BookRepositoryEF(BookDbContext bookDbContext)
        {
            BookDbContext = bookDbContext;

        }

        public async Task<Book> Add(Book book)
        {
            var result = await BookDbContext.Books.AddAsync(book);
            await BookDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> Delete(int id)
        {
            var book = await GetById(id);
            if (book == null)
            {
                return false;
            }

            BookDbContext.Books.Remove(book);
            await BookDbContext.SaveChangesAsync();
            return true;
        }

        public Task<List<Book>> GetAll()
        {
            return BookDbContext.Books.ToListAsync();
        }

        public Task<Book> GetById(int id)
        {
            return BookDbContext.Books.FindAsync(id);
        }

        public async Task<Book> Update(int id, Book book)
        {
            book.Id = id;
            var result = BookDbContext.Books.Update(book);
            await BookDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public Task<List<Book>> GetWithRatingHigherThan(int minRating)
        {
            return BookDbContext.Books.Where(b => b.Rating > minRating).ToListAsync();
        }
    }
}
