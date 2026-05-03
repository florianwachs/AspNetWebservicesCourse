using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Scalar.AspNetCore;
using TechConf.OData.Data;
using TechConf.OData.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

builder.Services.AddOpenApi();

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Event>("Events");
modelBuilder.EntitySet<Session>("Sessions");
modelBuilder.EntitySet<Speaker>("Speakers");
modelBuilder.EntitySet<Attendee>("Attendees");
modelBuilder.EntitySet<Registration>("Registrations");

modelBuilder.EntityType<Event>()
    .Function("AvailableSeats")
    .Returns<int>();

modelBuilder.EntityType<Event>()
    .Action("Cancel")
    .ReturnsFromEntitySet<Event>("Events");

builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
        .Expand()
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();
