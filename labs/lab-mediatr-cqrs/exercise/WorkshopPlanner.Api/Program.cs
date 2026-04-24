using FluentValidation;
using Scalar.AspNetCore;
using WorkshopPlanner.Api.Behaviors;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<WorkshopStore>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();

    // TODO: Task 5 - Register the logging behavior before validation so failed requests are still logged.
    // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWorkshopEndpoints();
app.MapDefaultEndpoints();

app.Run();
