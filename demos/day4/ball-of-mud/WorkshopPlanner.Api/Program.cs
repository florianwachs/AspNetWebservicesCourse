using Scalar.AspNetCore;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddSingleton<WorkshopStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWorkshopEndpoints();

app.Run();
