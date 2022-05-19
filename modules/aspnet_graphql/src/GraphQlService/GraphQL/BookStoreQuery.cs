using GraphQL;
using GraphQL.Types;
using GraphQL.MicrosoftDI;
using graphqlservice.Books;
using graphqlservice.GraphQL.Types;

namespace graphqlservice.GraphQL
{
    public class BookStoreQuery : ObjectGraphType
    {

        public BookStoreQuery()
        {
            Field<BookType>()
                .Name("book")
                .Argument<NonNullGraphType<IdGraphType>>("id")
                .Resolve()
                .WithScope()
                .WithService<IBookRepository>()
                .ResolveAsync(async (context, bookRepository) => await bookRepository.GetById(context.GetArgument<string>("id")));

            Field<ListGraphType<BookType>>()
                .Name("books")
                .Resolve()
                .WithScope() // creates a service scope as described above; not necessary for serial execution
                .WithService<IBookRepository>()
                .ResolveAsync(async (context, bookRepository) => await bookRepository.All());
        }
    }
}