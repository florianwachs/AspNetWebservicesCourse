using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.IoC.Autofac.DataAccess;
using WebAPI.IoC.Autofac.Models;

namespace WebAPI.IoC.Autofac.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        public IBookRepository BookRepository { get; set; }
        public BooksController(IBookRepository bookRepository)
        {
            BookRepository = bookRepository;
        }
        // GET api/books
        [Route("")]
        public IEnumerable<Book> GetBooks()
        {
            return BookRepository.GetAll();
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
