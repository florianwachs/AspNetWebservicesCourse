using GraphQL;
using GraphQL.Types;

namespace graphqlservice.GraphQL
{
    public class BookStoreSchema : Schema
    {
        public BookStoreSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<BookStoreQuery>();
            Mutation = resolver.Resolve<BookStoreMutation>();
        }
    }
}