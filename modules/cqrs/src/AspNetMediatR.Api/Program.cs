using AspNetMediatR.Api;
using AspNetMediatR.Api.Domain.Jokes;
using AspNetMediatR.Api.Endpoints;
using MediatR;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;
services.AddControllers();


services.AddJokesServices();
services.AddMediatR(cfg=>
{
    cfg.RegisterServicesFromAssemblyContaining<Joke>();
});

var app = builder.Build();

app.MapControllers();
JokeEndpointsV2.Map(app);

await app.RunAsync();