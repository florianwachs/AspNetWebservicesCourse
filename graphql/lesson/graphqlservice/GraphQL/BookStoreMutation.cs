using GraphQL.Types;
using graphqlservice.BookReviews;
using graphqlservice.Books;
using graphqlservice.GraphQL.Types;

namespace graphqlservice.GraphQL
{
    public class BookStoreMutation : ObjectGraphType
    {
        public BookStoreMutation(BookReviewRepository bookReviewRepository, BookRepository bookRepository)
        {
            // 👇 FieldAsync damit wir await im Resolver verwenden können
            FieldAsync<BookReviewType>("createReview", //                      👇 Unser InputType
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<BookReviewInputType>>() { Name = "review" }),
            resolve: async (context) =>
            {
            //                                    👇 Wir holen uns den InputType und lassen Ihn gleich von GraphQL.Net umwandeln
            var review = context.GetArgument<BookReview>("review");

            //                     👇 TryAsyncResolve fängt Exceptions ab und hängt diese als Error an die Response an
            return await context.TryAsyncResolve(async c => await bookReviewRepository.Add(review));
            });
        }
    }
}