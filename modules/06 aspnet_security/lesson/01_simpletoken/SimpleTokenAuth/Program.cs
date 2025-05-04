using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SimpleTokenAuth.DataAccess;
using SimpleTokenAuth.Domain;
using SimpleTokenAuth.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Bearer Authentication",
        Description = "Enter your Bearer token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Microsoft .NET"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddSingleton<DataProvider>();

// Configure Auth
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure DB
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapIdentityApi<AppUser>();

app.MapGet("/api/v1/authors", async Task<Ok<List<Author>>> (DataProvider provider) =>
    {
        var authors = await provider.GetAuthors();
        return TypedResults.Ok(authors);
    })
    .RequireAuthorization()
    .Produces(StatusCodes.Status500InternalServerError)
    .WithSummary("Returns all authors")
    .WithDescription("Additional Description / Examples")
    .WithTags("Get-Operations");

app.MapGet("/api/v1/authors/{id:int}",
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
    .WithName("GetAuthorById");

app.MapPost("/api/v1/authors",
        async Task<Results<Created<Author>, BadRequest>> (Author author, DataProvider provider, HttpContext httpContext,
            LinkGenerator linkGenerator) =>
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
    .WithTags("Post-Operations");

await EnsureMigratedDb(app);

app.Run();

async Task EnsureMigratedDb(WebApplication wapp)
{
    using var scope = wapp.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}