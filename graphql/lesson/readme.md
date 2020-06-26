# APIs mit GraphQL

GraphQL ist ein von Facebook definierter Ansatz zur Erstellung von Webschnittstellen. Im Gegensatz zu `REST` gibt es nur einen Endpunkt an den alle Abfragen (`Queries`) und Änderungen (`Mutations`) geschickt werden. Die Abfragen können dabei definieren, welche Daten von welchen Datenstrukturen benötigt werden. Somit lassen sich exakt die Daten für einen View abfragen und es können Round-Trips zum Server eingespart werden.

Den Code aus der Vorlesung findet Ihr [hier](graphqlservice/)

## Kleiner REST-Primer

- REpresentational State Transfer
- Datenmodel wird über Endpunkte und HTTP-Verben bereitgestellt
  - `GET`, `POST`, `PUT`, `PATCH`, `DELETE`

## Nachteile von REST

- Abruf von Objekt-Hierarchien == Mehrere Aufrufe der REST API
- Nicht selbsterklärende Schnittstelle, Tools wie OpenID (Swagger) werden benötigt
- Neue Funktionalität == vergrößerte API-Schnittstelle
- Filterung / Sortierung über Query-Parameters
  - kein Standard

## GraphQL

- ein Endpunkt für Query
- Schema das die abfragbaren Daten definiert
- Tools die das Schema interaktiv bereitstellen
- `GET` oder `POST` Request
- Immer eine Query mitgeliefert
- braucht nicht unbedingt HTTP

## GraphQL| Queries

```graphql
{
  #👇 Name der Query
  books {
    # Eigenschaften die abgerufen werden sollen
    name
    isbn
    # 👇 Können auch geschachtelt sein
    reviews {
      comment
      rating
    }
  }
}
```

## GraphQL| Queries

```graphql
{
  #👇 Query mit Argumenten
  book(id: "book1") {
    name
    isbn
    reviews {
      comment
      rating
    }
  }
}
```

## GraphQL| Queries

```graphql
{
  #👇 Es können meherer Abfragen in einer Query für unterschiedliche Typen gestellt werden
  b1: book(id: "book1") {
    ...bookFields
  }

  #                    👇 mit ...[fragment] wird ein Fragment referenziert
  b2: book(id: "book2") {
    ...bookFields
  }
}

# 👇 Mit Fragments können Listen von Feldern definiert werden
fragment bookFields on BookType {
  name
  price
  isbn
}
```

## GraphQL| Queries

```graphql
# Name Queries können parametrisiert werden. Das ! bedeutet das der Parameter required ist
query bookDetails($bookId: ID!) {
  book(id: $bookId) {
    name
    price
  }
}
```

## GraphQL| Queries

- Was ist mit Filtern / Sortieren / Paging?
- 👉 Nicht Teil der GraphQL spec. Muss vom Framework oder Dev selbst mit Argumenten umgesetzt werden

## GraphQL| Mutations

- Anlegen, Ändern und Löschen von Daten
- Müssen genauso wie Queries definiert werden und im Schema eingebunden sein

## GraphQL| Mutations

```graphql
# Mutationen können wie Queries einen Namen bekommen
mutation createReview($review: bookReviewInput!) {
  createReview(review: $review) {
    # Welche Werte wollen wir vom neuen Objekt
    id
    comment
  }
}
```

## GraphQL| Mutations

Der `Mutation` muss noch das Input-Objekt als Parameter übergeben werden

```json
{
  "review": { "bookId": "book1", "comment": "Cool", "rating": 5 }
}
```

## GraphQL| Mutations in Action

![Mutation Gif](assets/mutation.gif)

## GraphQL mit .NET

- GraphQL.NET (https://github.com/graphql-dotnet/graphql-dotnet)
- Hot Chocolate https://hotchocolate.io/

## GraphQL.NET

```powershell
dotnet add package GraphQL
dotnet add package GraphQL.Server.Transports.AspNetCore
dotnet add package GraphQL.Server.Ui.Playground
```

## GraphQL.NET

Wir brauchen:

- GraphQL-Types
- GraphQL-Query
- GraphQL-Schema

## GraphQL.NET| Types

- Leiten von `ObjectGraphType<T>` ab
- Mit `Field<T>` können Eigenschaften veröffentlicht werden z.B. `Field(book=>book.Isbn)`
- Können auch geschachtelte Daten enthalten

```csharp
Field<ListGraphType<BookReviewType>>("reviews", resolve: context=> bookReviewRepository.GetForBook(context.Source.Id));
```

## GraphQL.NET| Query

Die Query legt fest, was abgerufen werden kann

```csharp
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
```

## GraphQL.NET| Mutation

Mutations, als das Verändern / Anlegen von Daten funktioniert nach dem gleichen Prinzip wie Queries.
Mutations benötigen einen `InputGraphType`, welcher die benötigten Daten für die Änderung enthält.

## GraphQL.NET| InputType

```csharp
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
```

## GraphQL.NET| InputType

Eine `Mutation` ist ebenfalls ein `ObjectGraphType` wie `Query`

```csharp

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

```

## GraphQL.NET| Schema

Das Schema legt die Möglichkeiten des GraphQL Endpunktes fest

```csharp
public class BookStoreSchema : Schema
{
    public BookStoreSchema(IDependencyResolver resolver) : base(resolver)
    {
        Query = resolver.Resolve<BookStoreQuery>();
        Mutation = resolver.Resolve<BookStoreMutation>();
    }
}
```

## GraphQL.NET| ConfigureServices

Das Schema legt die Möglichkeiten des GraphQL Endpunktes fest

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Repositories registrieren
    services.AddScoped<BookRepository>();
    services.AddScoped<BookReviewRepository>();

    // GraphQL.NET verwendet eine eigene Resolver Abstraktion
    // Hier wird der AspNetCore-Resolver verwendet
    services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

    // Das Schema muss auch am DI registriert werden
    services.AddScoped<BookStoreSchema>();

    // Von GraphQL.NET benötigte Services hinzufügen, inkl. GraphTypes
    services.AddGraphQL(options =>
    {
        options.ExposeExceptions = true;
    }).AddGraphTypes(ServiceLifetime.Scoped);
    services.AddControllers();

    // FIXME: Workaround bis GraphQL.NET System.Text.Json verwendet
    services.Configure<IISServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });

    services.Configure<KestrelServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });
}
```

## GraphQL.NET| Configure

Das Schema legt die Möglichkeiten des GraphQL Endpunktes fest

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...

    // qraphql-Endpunkt registrieren
    // Man kann auch mehrere Schemas unter verschiedenen Endpunkten registrieren
    app.UseGraphQL<BookStoreSchema>();

    // Der Playground ist unter /ui/playground erreichbar und hilft beim
    // erforschen des Schemas und erstellen von abfragen
    app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
}

```

## GraphQL Clients

- GraphQL kann mit HTTP-Request angesprochen werden
- Es gibt Libraries die den Umgang mit GraphQL für .NET und JS vereinfachen

## GraphQL.Client

```powershell
dotnet add package GraphQL.Client
```

[GraphQL Client](https://github.com/graphql-dotnet/graphql-client)

## GraphQL Client JS

[Apollo Client](https://www.apollographql.com/docs/react/)

## Weitere GraphQL Feature

- Mutations
- Subscriptions

## Vorteile von GraphQL

- Automatische Validierung der Queries und Mutationen durch das GraphQL Schema
- Schema mit Tools wie Playground navigierbar
- Limitierung der Server-Roundtrips, eine Query kann alle Daten definieren die ein View benötigt
- Leichte Erweiterbarkeit der Schnittstelle, es gibt nur einen Endpunkt

## Nachteile von GraphQL

- Kein HTTP Caching
- Autorisierung komplizierter
- relativ neu -> bestpractices noch im Aufbau
- Queryoptimierung aufwendiger (n+1 Problem)
- Mehr Aufwand gegen Denial of Service Attacken

## Ressourcen
- https://github.com/graphql-dotnet/graphql-dotnet
- https://graphql-dotnet.github.io/docs/getting-started/introduction/
