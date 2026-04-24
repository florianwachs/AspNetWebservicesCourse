using SignalGame.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameState>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Clients", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Clients");

app.MapGet("/", () => Results.Ok(new { name = "Signal Showdown API", hub = "/hubs/game" }));
app.MapHub<GameHub>("/hubs/game");

app.Run();
