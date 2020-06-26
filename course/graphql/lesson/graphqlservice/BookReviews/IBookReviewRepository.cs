using System.Collections.Generic;
using System.Threading.Tasks;

namespace graphqlservice.BookReviews
{
    public interface IBookReviewRepository
    {
        Task<BookReview> Add(BookReview review);
        Task<IEnumerable<BookReview>> All();
        Task<BookReview> GetById(string id);
        Task<IEnumerable<BookReview>> GetForBook(string bookId);
    }
}