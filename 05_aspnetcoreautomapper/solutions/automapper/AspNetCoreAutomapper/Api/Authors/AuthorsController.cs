using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAutomapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreAutomapper.Api.Authors
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAll()
        {
            return Ok(await _authorRepository.GetAll());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Author>> UpdateAuthor(string id, [FromBody] Author author)
        {
            return Ok(await _authorRepository.Update(author));
        }
    }
}