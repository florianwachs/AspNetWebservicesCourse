using Microsoft.EntityFrameworkCore;
using TechConf.GraphQL.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

// TODO: Task 1 — Register the Hot Chocolate GraphQL server
// Register the GraphQL server with:
//   - AddQueryType<Query>()
//   - AddMutationType<Mutation>()
//   - AddFiltering()
//   - AddSorting()
//   - AddProjections()
//   - RegisterDbContextFactory<AppDbContext>()
// Hint: builder.Services.AddGraphQLServer()...

var app = builder.Build();

// Ensure database is created with seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// TODO: Task 1 — Map the GraphQL endpoint
// Hint: app.MapGraphQL();

app.Run();
