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
            Field<ListGraphType<NestedDataInputType>>("nestedData");
        }
    }

    public class NestedDataInputType: InputObjectGraphType
    {
        public NestedDataInputType()
        {
            Name = "nestedDataInput";
            Field<NonNullGraphType<StringGraphType>>("field1");
            Field<NonNullGraphType<StringGraphType>>("field2");

        }
    }
}