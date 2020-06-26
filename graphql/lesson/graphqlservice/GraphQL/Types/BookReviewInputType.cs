using GraphQL.Types;

namespace graphqlservice.GraphQL.Types
{
    public class BookReviewInputType : InputObjectGraphType
    {
        public BookReviewInputType()
        {
            Name = "bookReviewInput";
            Field<NonNullGraphType<StringGraphType>>("bookId");
            Field<NonNullGraphType<StringGraphType>>("comment");
            Field<IntGraphType>("rating");
        }
    }
}