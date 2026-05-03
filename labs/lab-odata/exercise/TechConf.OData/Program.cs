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

// TODO: Task 1 — Build the OData Entity Data Model (EDM)
// Create an ODataConventionModelBuilder and register EntitySets:
//   var modelBuilder = new ODataConventionModelBuilder();
//   modelBuilder.EntitySet<Event>("Events");
//   modelBuilder.EntitySet<Session>("Sessions");
//   modelBuilder.EntitySet<Speaker>("Speakers");
//   modelBuilder.EntitySet<Attendee>("Attendees");
//   modelBuilder.EntitySet<Registration>("Registrations");
//
// TODO: Task 4 — Register OData Function
//   modelBuilder.EntityType<Event>()
//       .Function("AvailableSeats")
//       .Returns<int>();
//
// TODO: Task 5 — Register OData Action
//   modelBuilder.EntityType<Event>()
//       .Action("Cancel")
//       .ReturnsFromEntitySet<Event>("Events");

// TODO: Task 1 — Register OData services
// builder.Services.AddControllers()
//     .AddOData(options => options
//         .Select()
//         .Filter()
//         .OrderBy()
//         .SetMaxTop(100)
//         .Count()
//         .Expand()
//         .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

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

// TODO: Task 1 — Map controllers (required for OData)
// app.MapControllers();

app.Run();
