using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using restservice.Books;

namespace restservice.BookReviews
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookReviewsController : ControllerBase
    {
        public BookReviewsController(ILogger<BookReviewsController> logger, BookReviewRepository bookReviewRepository)
        {
            Logger = logger;
            BookReviewRepository = bookReviewRepository;
        }

        public ILogger<BookReviewsController> Logger { get; }
        public BookReviewRepository BookReviewRepository { get; }

        [HttpGet]
        public Task<IEnumerable<BookReview>> Get()
        {
            return BookReviewRepository.All();
        }

        [HttpPost]
        public Book CreateBookReview(BookReview data)
        {
            throw new InvalidOperationException();
        }

    }
}