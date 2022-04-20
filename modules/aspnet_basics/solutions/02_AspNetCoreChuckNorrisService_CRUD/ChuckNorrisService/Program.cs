using ChuckNorrisService;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJokeProvider, FileSystemJokeProvider>();

var app = builder.Build();

JokesEndpoints.Register(app);

app.Run();