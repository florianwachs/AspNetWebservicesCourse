using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace graphqlservice.BookReviews
{
    public class InMemoryBookReviewRepository : IBookReviewRepository
    {
        private static int fakeId = 0;
        private static readonly List<BookReview> _bookReviews = new List<BookReview>()
    {
        new BookReview{ Id=GetNewId(), BookId = "book1", Comment="Awesome", Rating=5},
        new BookReview{ Id=GetNewId(), BookId = "book1", Comment="Good content", Rating=4},
        new BookReview{ Id=GetNewId(), BookId = "book2", Comment="Life changing", Rating=5},
    };

        private static string GetNewId()
        {
            return Interlocked.Increment(ref fakeId).ToString();
        }
        public Task<IEnumerable<BookReview>> All() => Task.FromResult((IEnumerable<BookReview>)_bookReviews);

        public Task<IEnumerable<BookReview>> GetForBook(string bookId) => Task.FromResult(_bookReviews.Where(review => review.BookId == bookId));

        public Task<BookReview> Add(BookReview review)
        {
            review.Id = GetNewId();
            //TODO: Validation ob es das Buch überhaupt gibt
            _bookReviews.Add(review);
            return Task.FromResult(review);
        }

        public Task<BookReview> GetById(string id) => Task.FromResult(_bookReviews.FirstOrDefault(review => review.Id == id));

    }
}