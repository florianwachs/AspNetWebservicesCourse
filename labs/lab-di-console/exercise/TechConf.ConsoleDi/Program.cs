using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechConf.ConsoleDi;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

// TODO Task 1: Register the remaining services:
// builder.Services.AddTransient<OperationMarker>();
// builder.Services.AddScoped<WorkshopScope>();
// builder.Services.AddTransient<AgendaPrinter>();

using var app = builder.Build();

Console.WriteLine("TechConf DI console lab");
Console.WriteLine("TODO: Register the services above, then create two scopes and resolve AgendaPrinter inside them.");
Console.WriteLine("Expected result: the transient marker changes every resolve, while the scoped marker stays the same within a scope.");

// TODO Task 2: Create scope #1 and resolve AgendaPrinter twice.
// TODO Task 3: Create scope #2 and resolve AgendaPrinter once.
