using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreSwagger.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSwagger.Api.Authors
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        /// <summary>
        /// Gets all authors
        /// </summary>
        /// <returns>All authors</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAll()
        {
            return Ok(await _authorRepository.GetAll());
        }
    }
}