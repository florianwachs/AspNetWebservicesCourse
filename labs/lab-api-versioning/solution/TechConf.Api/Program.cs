using Asp.Versioning;
using Scalar.AspNetCore;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
    {
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
    app.MapScalarApiReference(options =>
    {
        var descriptions = app.DescribeApiVersions();

        for (var i = 0; i < descriptions.Count; i++)
        {
            var description = descriptions[i];
            var isDefault = i == descriptions.Count - 1;

            options.AddDocument(description.GroupName, description.GroupName, isDefault: isDefault);
        }
    });
}

app.MapVersionedEventEndpoints();

app.Run();
