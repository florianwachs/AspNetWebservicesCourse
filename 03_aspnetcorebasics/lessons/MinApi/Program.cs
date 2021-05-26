using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
await using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

InMemoryJokeRepository repository = new();

app.MapGet("/", (Func<Task<IReadOnlyCollection<Joke>>>)(() => repository.All()));
app.MapGet("/{id}", (Func<string, Task<Joke>>)((id) => repository.GetById(id)));
app.MapGet("/random", (Func<string, Task<Joke>>)((id) => repository.GetRandomJoke()));
app.MapDelete("/{id}", (Func<string, Task>)(async (id) => await repository.Delete(id)));

await app.RunAsync();