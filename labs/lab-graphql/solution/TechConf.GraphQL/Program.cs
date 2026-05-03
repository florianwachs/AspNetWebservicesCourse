using Microsoft.EntityFrameworkCore;
using TechConf.GraphQL.Data;
using TechConf.GraphQL.GraphQL;
using TechConf.GraphQL.GraphQL.DataLoaders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"), ServiceLifetime.Scoped);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<SessionResolvers>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .RegisterDbContextFactory<AppDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGraphQL();

app.Run();
