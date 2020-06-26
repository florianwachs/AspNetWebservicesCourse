using GraphQL.Types;
using graphqlservice.Books;
using graphqlservice.GraphQL.Types;

namespace graphqlservice.GraphQL
{
    public class BookStoreQuery : ObjectGraphType
    {

        public BookStoreQuery(BookRepository bookRepository)
        {
            Field<ListGraphType<BookType>>(
                "books",
                resolve: context => bookRepository.All()
            );

            Field<BookType>("book",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
            resolve: context =>
            {
                var id = context.GetArgument<string>("id");
                return bookRepository.GetById(id);
            });

        }
    }
}