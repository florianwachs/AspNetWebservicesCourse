using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;
using TechConf.Api.Exceptions;
using TechConf.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// AddDbContext registers AppDbContext as scoped, which is the right lifetime for one HTTP request.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

await DbSeeder.MigrateAndSeedAsync(app.Services);

app.MapGet("/", () => TypedResults.Ok(new
{
    Message = "TechConf day 2 sample is running.",
    Database = "PostgreSQL",
    Docs = "/scalar/v1"
}))
.WithTags("Meta")
.WithSummary("Show basic sample information");

// This makes it easy to demonstrate a 500 ProblemDetails response during the lecture.
app.MapGet("/api/debug/fail", ThrowDemoException)
    .WithTags("Debug")
    .WithSummary("Throw a demo exception");

app.MapEventEndpoints();

app.Run();

static IResult ThrowDemoException()
{
    throw new InvalidOperationException("This endpoint intentionally throws an exception.");
}
