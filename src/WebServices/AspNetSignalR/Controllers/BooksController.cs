using AspNetSignalR.Hubs;
using AspNetSignalR.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace AspNetSignalR.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private static int lastBookId = 4;

        // ACHTUNG: nur zu DEMO-Zwecken. Dictionary ist nicht threadsafe!
        private static readonly Dictionary<int, Book> books = new Dictionary<int, Book>() 
        {
            {1,new Book(1,"1430242337", "C# Pro", 30, new []{"Troelson"}, DateTime.Now.AddYears(-2))},
            {2,new Book(2,"161729134X", "C# in Depth", 40, new []{"Skeet"}, DateTime.Now.AddMonths(-2))},
            {3,new Book(3,"1449320104", "C# in a Nutshell", 40, new []{"Albahari"}, DateTime.Now.AddMonths(-2))},
            {4,new Book(4,"0596807260", "Entity Framework 6", 20, new []{"Lerman"}, DateTime.Now.AddMonths(-2))},
        };

        private IHubContext<IBookClient> HubContext { get; set; }

        public BooksController()
        {
            HubContext = GlobalHost.ConnectionManager.GetHubContext<BookHub, IBookClient>();            
        }

        // GET api/books
        [Route("")]
        public IEnumerable<Book> GetBooks()
        {
            return books.Values;
        }

        // GET api/books/1
        [Route("{id:int}")]
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBookById(int id)
        {
            Book book;
            return books.TryGetValue(id, out book) ? (IHttpActionResult)Ok(book) : NotFound();
        }

        // ~ überschreibt das RoutePrefix
        // GET api/authors/skeet/books
        [Route("~/api/authors/{author:alpha}/books")]
        public IEnumerable<Book> GetBookByAuthorName(string author)
        {
            var result = books.Values
                .Where(b => b.Authors != null && b.Authors.Contains(author, StringComparer.InvariantCultureIgnoreCase))
                .ToArray();
            return result;
        }

        [Route("~/api/authors/{author:alpha}/books/{year:int:min(1950):max(2050)}")]
        // GET api/authors/skeet/books/2015
        public IEnumerable<Book> GetBookByAuthorNameInYear(string author, int year)
        {
            var result = books.Values
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year == year
                    && b.Authors != null && b.Authors.Contains(author, StringComparer.InvariantCultureIgnoreCase))
                    .ToArray();
            return result;
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
            book.Id = GetNextId();
            books.Add(book.Id, book);

            HubContext.Clients.All.bookUpdate(book, UpdateKinds.Created);

            return Ok(book);
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

            if (!books.ContainsKey(id))
            {
                return NotFound();
            }
            book.Id = id;
            books[id] = book;
            HubContext.Clients.All.bookUpdate(book, UpdateKinds.Updated);
            return Ok(book);
        }

        private int GetNextId()
        {
            return ++lastBookId;
        }

    }
}
