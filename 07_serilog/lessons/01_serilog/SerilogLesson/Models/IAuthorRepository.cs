using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerilogLesson.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}