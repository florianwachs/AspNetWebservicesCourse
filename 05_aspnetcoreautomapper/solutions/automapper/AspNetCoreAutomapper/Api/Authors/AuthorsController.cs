using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAutomapper.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreAutomapper.Api.Authors
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorVm>>> GetAll()
        {
            var authors = await _authorRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<AuthorVm>>(authors));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Author>> UpdateAuthor(string id, [FromBody] Author author)
        {
            return Ok(await _authorRepository.Update(author));
        }
    }
}