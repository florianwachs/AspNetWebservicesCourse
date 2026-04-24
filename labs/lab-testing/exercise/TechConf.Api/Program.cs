using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("eventdb");

var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

var events = app.MapGroup("/api/events");

events.MapGet("/", async (AppDbContext db) =>
    TypedResults.Ok(await db.Events.OrderBy(e => e.Date).ToListAsync()));

events.MapGet("/{id:guid}", async (Guid id, AppDbContext db) =>
    await db.Events.FindAsync(id) is { } ev
        ? TypedResults.Ok(ev)
        : Results.NotFound());

events.MapPost("/", async (Event ev, AppDbContext db) =>
{
    ev.Id = Guid.NewGuid();
    db.Events.Add(ev);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/api/events/{ev.Id}", ev);
});

events.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var ev = await db.Events.FindAsync(id);
    if (ev is null) return Results.NotFound();
    db.Events.Remove(ev);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public partial class Program { }
