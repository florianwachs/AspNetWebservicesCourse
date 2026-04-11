using AspireOpenTelemetry.Server.Tariffs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<TariffDbContext>("tariffsdb");
builder.Services.AddScoped<TariffService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var tariffs = app.MapGroup("/api/tariffs")
    .WithTags("Tariffs");

tariffs.MapGet("/", async (TariffService tariffService, CancellationToken cancellationToken) =>
        TypedResults.Ok(await tariffService.GetCurrentBatchAsync(cancellationToken)))
    .WithName("GetCurrentTariffs")
    .WithSummary("Get the current absurd tariff board.");

tariffs.MapPost("/generate", async (GenerateTariffsRequest? request, TariffService tariffService, CancellationToken cancellationToken) =>
        TypedResults.Ok(await tariffService.GenerateBatchAsync(request?.Count ?? 8, request?.Scenario, cancellationToken)))
    .WithName("GenerateTariffs")
    .WithSummary("Generate a fresh batch of ridiculous tariffs.");

tariffs.MapGet("/{countryCode}", async Task<Results<Ok<TariffInspectionResponse>, NotFound>> (string countryCode, TariffService tariffService, CancellationToken cancellationToken) =>
{
    var inspection = await tariffService.InspectCountryAsync(countryCode, cancellationToken);
    return inspection is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(inspection);
})
.WithName("InspectTariff")
.WithSummary("Inspect a single country's tariff story.");

tariffs.MapPost("/audit", async (TariffAuditRequest? request, TariffService tariffService, CancellationToken cancellationToken) =>
        TypedResults.Ok(await tariffService.RunAuditAsync(request?.FocusCountryCode, cancellationToken)))
    .WithName("AuditTariffs")
    .WithSummary("Run an emergency tariff audit.");

tariffs.MapPost("/disputes", async (TradeDisputeRequest? request, TariffService tariffService, CancellationToken cancellationToken) =>
        TypedResults.Ok(await tariffService.ResolveDisputeAsync(request?.CountryCode, request?.Complaint, cancellationToken)))
    .WithName("ResolveTradeDispute")
    .WithSummary("Stage a funny trade dispute to create telemetry.");

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TariffDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.MapDefaultEndpoints();

if (app.Environment.WebRootPath is { Length: > 0 } webRootPath && Directory.Exists(webRootPath))
{
    app.UseFileServer();
}

app.Run();
