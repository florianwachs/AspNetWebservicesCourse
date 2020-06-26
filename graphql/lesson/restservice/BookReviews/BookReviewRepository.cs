using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace restservice.BookReviews
{
    public class BookReviewRepository
    {
        private static readonly List<BookReview> _bookReviews = new List<BookReview>()
    {
        new BookReview{ Id="1", BookId = "book1", Comment="Awesome", Rating=5},
        new BookReview{ Id="2", BookId = "book1", Comment="Good content", Rating=4},
        new BookReview{ Id="2", BookId = "book2", Comment="Life changing", Rating=5},
    };
        public Task<IEnumerable<BookReview>> All() => Task.FromResult((IEnumerable<BookReview>)_bookReviews);

        public Task<IEnumerable<BookReview>> GetForBook(string bookId) => Task.FromResult(_bookReviews.Where(review => review.BookId == bookId));

        public Task<BookReview> GetById(string id) => Task.FromResult(_bookReviews.FirstOrDefault(review => review.Id == id));

    }
}