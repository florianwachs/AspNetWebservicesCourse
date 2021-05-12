﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreMicroservices.Books.Api.DataAccess;
using AspNetCoreMicroservices.Books.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMicroservices.Books.Api.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext _dbContext;

        public BookRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Book> Add(Book book)
        {
            EnsureId(book);
            var result = await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
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

        public async Task<Book> GetById(string id)
        {
            return await _dbContext.Books.FindAsync(id);
        }

        public async Task<Book> Update(Book book)
        {
            var updated = _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return updated.Entity;
        }
    }
}
