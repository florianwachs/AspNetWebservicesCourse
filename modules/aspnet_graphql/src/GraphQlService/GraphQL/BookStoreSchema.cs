using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace graphqlservice.GraphQL
{
    public class BookStoreSchema : Schema
    {
        public BookStoreSchema(IServiceProvider resolver) : base(resolver)
        {
            Query = resolver.GetRequiredService<BookStoreQuery>();
            Mutation = resolver.GetRequiredService<BookStoreMutation>();
        }
    }
}