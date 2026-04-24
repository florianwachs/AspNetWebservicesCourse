using Akka.Hosting;
using Scalar.AspNetCore;
using TechConf.Akka.Api.Actors;
using TechConf.Akka.Api.Endpoints;
using TechConf.Akka.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddAkka("TechConfReservations", akkaBuilder =>
{
    akkaBuilder.WithActors((system, registry) =>
    {
        var seatReservations = system.ActorOf(
            SeatReservationsActor.Create(SessionCatalog.All),
            "seat-reservations");

        registry.TryRegister<SeatReservationsActorKey>(seatReservations);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Ok(new
{
    name = "TechConf Akka API",
    docs = "/scalar/v1",
    sessions = "/api/sessions"
}));

app.MapReservationEndpoints();
app.MapDefaultEndpoints();

app.Run();
