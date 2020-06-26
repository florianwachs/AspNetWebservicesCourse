using GraphQL.Types;
using graphqlservice.BookReviews;
using graphqlservice.Books;

namespace graphqlservice.GraphQL.Types
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType(BookReviewRepository bookReviewRepository)
        {
            Field(b => b.Id);
            Field(b => b.Isbn);
            Field(b => b.Name);
            Field(b => b.Price);
            Field<ListGraphType<BookReviewType>>("reviews", resolve: context=> bookReviewRepository.GetForBook(context.Source.Id));
        }
    }
}