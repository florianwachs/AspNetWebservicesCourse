using Scalar.AspNetCore;
using TechConf.Sessions.Api.Data;
using TechConf.Sessions.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => TypedResults.Ok(new
{
    message = "TechConf Sessions API is running.",
    openApi = "/openapi/v1.json",
    scalar = "/scalar/v1",
    resource = "/api/sessions"
}))
    .WithName("GetApiInfo")
    .WithSummary("Show entry points for the demo API");

app.MapSessionEndpoints();

app.Run();
