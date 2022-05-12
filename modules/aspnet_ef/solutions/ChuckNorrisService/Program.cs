using ChuckNorrisService;
using ChuckNorrisService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

bool useInMemory = false;
builder.Services.AddJokesServices(builder.Configuration, useInMemory);

var app = builder.Build();

await new JokeDbSeeder().SeedDb(app.Services, useInMemory);

JokesEndpoints.Register(app);

await app.RunAsync();