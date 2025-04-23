using AspNetOpenApi.Domain;
using AspNetOpenApi.Providers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 👇Dependencies für Swagger am Dependency Injection Container registrieren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Sample API",
        Description = "Sample",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});
builder.Services.AddSingleton<DataProvider>();

var app = builder.Build();

// 👇Dieser Codeblock registriert Swagger nur, wenn es sich um die Entwicklungsumgebung handelt
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//Lesson1(app);
//Lesson2(app);
//Lesson3(app);
Lesson4(app);

app.Run();

#region Lesson 1

void Lesson1(WebApplication webApplication)
{
    webApplication.MapGet("/api/v1/authors", async (DataProvider provider) =>
    {
        var authors = await provider.GetAuthors();
        return authors;
    }).WithOpenApi();

    webApplication.MapGet("/api/v1/authors/{id}", async (int id, DataProvider provider) =>
    {
        var author = await provider.GetAuthorById(id);
        if (author == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(author);
    }).WithOpenApi();
}

#endregion

#region Lesson 2

void Lesson2(WebApplication webApplication)
{
    webApplication.MapGet("/api/v1/authors", async (DataProvider provider) =>
        {
            var authors = await provider.GetAuthors();
            return authors;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Returns all authors")
        .WithOpenApi();

    webApplication.MapGet("/api/v1/authors/{id:int}", async (int id, DataProvider provider) =>
        {
            var author = await provider.GetAuthorById(id);
            if (author == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(author);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Returns an author by id")
        .WithOpenApi();
}

#endregion

#region Lesson 3

void Lesson3(WebApplication webApplication)
{
    webApplication.MapGet("/api/v1/authors", async Task<Ok<List<Author>>> (DataProvider provider) =>
        {
            var authors = await provider.GetAuthors();
            return TypedResults.Ok(authors);
        })
        //.Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Returns all authors")
        .WithDescription("Additional Description / Examples")
        .WithOpenApi();

    webApplication.MapGet("/api/v1/authors/{id:int}",
            async Task<Results<Ok<Author>, NotFound>> (int id, DataProvider provider) =>
            {
                var author = await provider.GetAuthorById(id);
                if (author == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(author);
            })
        //.Produces(StatusCodes.Status200OK)
        //.Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Returns an author by id")
        .WithDescription("Additional Description / Examples")
        .WithOpenApi();
}

#endregion

#region Lesson 4

void Lesson4(WebApplication webApplication)
{
    webApplication.MapGet("/api/v1/authors", async Task<Ok<List<Author>>> (DataProvider provider) =>
        {
            var authors = await provider.GetAuthors();
            return TypedResults.Ok(authors);
        })
        .Produces(StatusCodes.Status500InternalServerError)
        //.WithSummary("Returns all authors")
        //.WithDescription("Additional Description / Examples")
        .WithTags("Get-Operations")
        .WithOpenApi(op =>
        {
            op.Summary = "Returns all authors";
            op.Description = "Additional Description / Examples";

            return op;
        });

    webApplication.MapGet("/api/v1/authors/{id:int}",
            async Task<Results<Ok<Author>, NotFound>> (int id, DataProvider provider) =>
            {
                var author = await provider.GetAuthorById(id);
                if (author == null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(author);
            })
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags("Get-Operations")
        .WithOpenApi(op =>
        {
            op.Summary = "Returns an author by id";
            op.Description = "Additional Description / Examples";
            
            var idParam = op.Parameters[0];            
            idParam.Required = true;
            idParam.Description = "The Id of the author";
     
            return op;
        })
        .WithName("GetAuthorById");
    
    webApplication.MapPost("/api/v1/authors",
            async Task<Results<Created<Author>, BadRequest>> (Author author, DataProvider provider, HttpContext httpContext, LinkGenerator linkGenerator) =>
            {
                // Code to add an author
                if (Random.Shared.NextDouble() > 0.5)
                {
                    return TypedResults.BadRequest();
                }

                var uriToCreatedAuthor = linkGenerator.GetUriByName(httpContext, "AuthorById");

                return TypedResults.Created(uriToCreatedAuthor, author);
            })
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags("Post-Operations")
        .WithOpenApi(op =>
        {
            op.Summary = "Creates a new author";
            op.Description = "Additional Description / Examples";
            var bodyDescription = op.RequestBody;
            bodyDescription.Description = "The data for the new author";
            bodyDescription.Required = true;

            return op;
        });
}

#endregion