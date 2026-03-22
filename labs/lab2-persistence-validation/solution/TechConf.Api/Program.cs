using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;
using TechConf.Api.Exceptions;
using TechConf.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

await DbSeeder.MigrateAndSeedAsync(app.Services);

app.MapEventEndpoints();

app.Run();
