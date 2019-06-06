using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreAutomapper.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
        Task<Author> Update(Author author);
        Task<Author> GetById(Guid id);
    }
}