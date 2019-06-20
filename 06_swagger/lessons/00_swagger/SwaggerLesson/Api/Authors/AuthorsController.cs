using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwaggerLesson.Models;

namespace SwaggerLesson.Api.Authors
{
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _authorRepository.GetAll());
        }
    }
}