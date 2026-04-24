using FluentValidation;
using Scalar.AspNetCore;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<WorkshopStore>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWorkshopEndpoints();

app.Run();
