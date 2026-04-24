using FluentValidation;
using Scalar.AspNetCore;
using WorkshopPlanner.Api.Features.Workshops;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.CreateWorkshop;
using WorkshopPlanner.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IWorkshopRepository, InMemoryWorkshopRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkshopValidator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateWorkshopHandler>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWorkshopEndpoints();

app.Run();
