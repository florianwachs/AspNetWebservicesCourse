using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreAutomapper.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
        Task<IReadOnlyCollection<Author>> GetAllWithJokes();
        Task<Author> Update(Author author);
        Task<Author> GetById(Guid id);
    }
}