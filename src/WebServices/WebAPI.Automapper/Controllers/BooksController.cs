using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Automapper.DataAccess;
using WebAPI.Automapper.Models;
using WebAPI.Automapper.ViewModels;

namespace WebAPI.Automapper.Controllers
{
    public class BooksController : ApiController
    {
        public IBookRepository BookRepository { get; set; }
        public IMapper Mapper { get; set; }
        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            BookRepository = bookRepository;
            Mapper = mapper;
        }
        // GET api/books
        [Route("")]
        public IEnumerable<BookViewModel> GetBooks()
        {
            var mapped = Mapper.Map<IEnumerable<BookViewModel>>(BookRepository.GetAll());
            return mapped;
        }

        // GET api/books/1
        [Route("{id:int}", Name = "BookById")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBookById(int id)
        {
            var book = BookRepository.GetById(id);
            return book != null ? (IHttpActionResult)Ok(book) : NotFound();
        }

        [Route("")]
        [HttpPost]
        [ResponseType(typeof(Book))]
        public IHttpActionResult CreateBook(Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var created = BookRepository.Add(book);
            return Created(Url.Link("BookById", new { Id = created.Id }), created);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult UpdateBook(int id, Book book)
        {
            // Während des Model Bindings werden die Validatoren
            // von Book geprüft
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existing = BookRepository.GetById(id);

            if (existing == null)
            {
                return NotFound();
            }

            book = BookRepository.Update(id, book);
            return Ok(book);
        }

    }
}