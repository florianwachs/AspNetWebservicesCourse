using System;
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
        public async Task<ActionResult<IEnumerable<AuthorWithJokesVm>>> GetAll()
        {
            var authors = await _authorRepository.GetAllWithJokes();
            return Ok(_mapper.Map<IEnumerable<AuthorWithJokesVm>>(authors));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuthorVm>> UpdateAuthor(Guid id, [FromBody] AuthorEditVm update)
        {
            var existingAuthor = await _authorRepository.GetById(id);
            if (existingAuthor == null)
                return NotFound();

            var updated = _mapper.Map(update, existingAuthor);
            
            return Ok(_mapper.Map<AuthorVm>(await _authorRepository.Update(updated)));
        }
    }
}