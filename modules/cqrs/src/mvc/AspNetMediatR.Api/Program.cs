using AspNetMediatR.Api.Domain.Jokes.Models;
using AspNetMediatR.Api;
using MediatR;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;
services.AddControllers();


services.AddJokesServices();
services.AddMediatR(typeof(Joke).Assembly);
var app = builder.Build();

app.MapControllers();

await app.RunAsync();