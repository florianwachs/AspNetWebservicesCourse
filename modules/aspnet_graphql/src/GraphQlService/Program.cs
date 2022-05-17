
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using graphqlservice.BookReviews;
using graphqlservice.Books;
using graphqlservice.GraphQL;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Repositories registrieren
services.AddScoped<IBookRepository, InMemoryBookRepository>();
services.AddScoped<IBookReviewRepository, InMemoryBookReviewRepository>();
// Das Schema muss auch am DI registriert werden
//services.AddScoped<BookStoreSchema>();

// Von GraphQL.NET benötigte Services hinzufügen, inkl. GraphTypes
services.AddGraphQL(o =>
{
    o
    //.AddServer(true)
    .AddHttpMiddleware<BookStoreSchema>()
    .AddSchema<BookStoreSchema>()
       .AddSystemTextJson()
       .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
       .AddGraphTypes(typeof(Book).Assembly);
});
//.AddSystemTextJson()
//.AddGraphTypes(ServiceLifetime.Scoped);
services.AddControllers();
services.AddHttpContextAccessor();
var app = builder.Build();

app.UseRouting();

// graphql-Endpunkt registrieren
// Man kann auch mehrere Schemas unter verschiedenen Endpunkten registrieren
app.UseGraphQL<BookStoreSchema>();

// Der Playground ist unter /ui/playground erreichbar und hilft beim
// erforschen des Schemas und erstellen von abfragen
app.UseGraphQLPlayground();


app.Run();
