using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using restservice.BookReviews;

namespace restservice.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        public BooksController(ILogger<BooksController> logger, BookRepository bookRepository, BookReviewRepository bookReviewRepository)
        {
            Logger = logger;
            BookRepository = bookRepository;
            BookReviewRepository = bookReviewRepository;
        }

        public ILogger<BooksController> Logger { get; }
        public BookRepository BookRepository { get; }
        public BookReviewRepository BookReviewRepository { get; }

        [HttpGet]
        public Task<IEnumerable<Book>> Get()
        {
            return BookRepository.All();
        }

        [HttpGet("{id}")]
        public Task<Book> GetBookById(string id)
        {
            return BookRepository.GetById(id);
        }

        [HttpGet("{id}/reviews")]
        public Task<IEnumerable<BookReview>> GetReviewsForBook(string id)
        {
            return BookReviewRepository.GetForBook(id);
        }

        [HttpPost]
        public Book CreateBook(Book data)
        {
            throw new InvalidOperationException();
        }

        [HttpPut("{id}")]
        public Book UpdateBook(Book data)
        {
            throw new InvalidOperationException();
        }
    }
}