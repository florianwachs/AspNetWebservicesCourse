var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/config", (IConfiguration configuration) =>
{
    var apiBaseUrl = configuration["services:api:https:0"]
        ?? configuration["services:api:http:0"]
        ?? configuration["ApiBaseUrl"]
        ?? "https://localhost:7111";

    return Results.Ok(new { hubUrl = $"{apiBaseUrl.TrimEnd('/')}/hubs/game" });
});

app.Run();
