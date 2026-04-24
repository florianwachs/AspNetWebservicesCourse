using Scalar.AspNetCore;
using WorkshopPlanner.Api.Endpoints;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Workshops.AddSession;
using WorkshopPlanner.Application.Workshops.CreateWorkshop;
using WorkshopPlanner.Application.Workshops.GetWorkshopById;
using WorkshopPlanner.Application.Workshops.GetWorkshops;
using WorkshopPlanner.Application.Workshops.PublishWorkshop;
using WorkshopPlanner.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IWorkshopRepository, InMemoryWorkshopRepository>();
builder.Services.AddTransient<GetWorkshopsHandler>();
builder.Services.AddTransient<GetWorkshopByIdHandler>();
builder.Services.AddTransient<CreateWorkshopHandler>();
builder.Services.AddTransient<AddSessionHandler>();
builder.Services.AddTransient<PublishWorkshopHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWorkshopEndpoints();

app.Run();
