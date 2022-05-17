using GraphQL.Types;
using graphqlservice.BookReviews;

namespace graphqlservice.GraphQL.Types
{
    public class BookReviewType : ObjectGraphType<BookReview>
    {
        public BookReviewType()
        {
            Field(r => r.Id);
            Field(r => r.Comment);
            Field(r => r.Rating);
        }

    }
}