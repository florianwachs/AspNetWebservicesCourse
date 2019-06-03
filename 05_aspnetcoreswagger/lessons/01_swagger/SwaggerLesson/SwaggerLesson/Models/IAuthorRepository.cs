using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwaggerLesson.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}