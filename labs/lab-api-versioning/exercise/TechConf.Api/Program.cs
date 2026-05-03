using Scalar.AspNetCore;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// TODO: Task 1 - Replace this simple OpenAPI registration with:
// builder.Services.AddApiVersioning(...)
//     .AddApiExplorer(...)
//     .AddOpenApi();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // TODO: Task 5 - Replace this with:
    // app.MapOpenApi().WithDocumentPerVersion();
    // app.MapScalarApiReference(options => { ... });
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// TODO: Task 2 - Once versioning is configured, switch to:
// app.MapVersionedEventEndpoints();
app.MapEventEndpoints();

app.Run();
