using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Types;
using graphqlservice.BookReviews;
using graphqlservice.GraphQL.Types;

namespace graphqlservice.GraphQL
{
    public class BookStoreMutation : ObjectGraphType
    {
        public BookStoreMutation()
        {
            Field<BookReviewType>()
                .Name("createReview")
                .Argument<NonNullGraphType<BookReviewInputType>>("review")
                .Resolve()
                .WithScope()
                .WithService<IBookReviewRepository>()
                .ResolveAsync(async (context, bookReviewRepository) =>
                    {
                    //                                    👇 Wir holen uns den InputType und lassen Ihn gleich von GraphQL.Net umwandeln
                    var review = context.GetArgument<BookReview>("review");
                    return await bookReviewRepository.Add(review);
                    });
        }
    }
}