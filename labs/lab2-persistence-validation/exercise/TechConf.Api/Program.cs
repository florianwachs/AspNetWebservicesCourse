using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;
using TechConf.Api.Exceptions;
using TechConf.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// TODO: Task 2 - Register AppDbContext with PostgreSQL
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// TODO: Task 4 - Register EventRepository for dependency injection
// builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// TODO: Task 6 - Register GlobalExceptionHandler
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// TODO: Task 6 - Add exception handler and status code pages middleware
// app.UseExceptionHandler();
// app.UseStatusCodePages();

// TODO: Stretch Goal 2 - After Task 2 is complete and your first migration exists,
// uncomment this to apply migrations on startup and insert the sample data once.
// await DbSeeder.MigrateAndSeedAsync(app.Services);

app.MapEventEndpoints();

app.Run();
