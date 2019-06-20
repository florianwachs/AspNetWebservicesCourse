using AspNetCoreMicroservices.Frontend.ApiClients;
using AspNetCoreMicroservices.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Frontend.ApiGateway
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            return Ok(await _booksService.GetBooks());
        }
    }
}
