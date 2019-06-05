using System.Threading.Tasks;
using AspNetCoreSwagger.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSwagger.Api.Authors
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