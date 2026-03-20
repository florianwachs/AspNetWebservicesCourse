using Scalar.AspNetCore;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Keeping one tiny inline endpoint makes it easier to compare the starting point
// with the extracted CRUD endpoints below.
app.MapGet("/", () => "Hello, World!");

app.MapEventEndpoints();

app.Run();
