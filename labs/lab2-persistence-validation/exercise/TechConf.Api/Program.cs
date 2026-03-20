using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;
using TechConf.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// TODO: Task 2 - Register AppDbContext with PostgreSQL
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapEventEndpoints();

app.Run();
