using AspNetCore.EFRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.EFRepository.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAll();
        Task<Book> GetById(int id);
        Task<Book> Add(Book p);
        Task<Book> Update(int id, Book p);
        Task<bool> Delete(int id);
        Task<List<Book>> GetWithRatingHigherThan(int minRating);
    }
}
