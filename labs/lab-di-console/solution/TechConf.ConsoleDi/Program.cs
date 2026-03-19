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
builder.Services.AddTransient<OperationMarker>();
builder.Services.AddScoped<WorkshopScope>();
builder.Services.AddTransient<AgendaPrinter>();

using var app = builder.Build();

Console.WriteLine("TechConf DI console lab");

using (var scope = app.Services.CreateScope())
{
    var printer = scope.ServiceProvider.GetRequiredService<AgendaPrinter>();
    printer.Print("Minimal APIs");

    var secondPrinter = scope.ServiceProvider.GetRequiredService<AgendaPrinter>();
    secondPrinter.Print("Dependency Injection");
}

using (var scope = app.Services.CreateScope())
{
    var printer = scope.ServiceProvider.GetRequiredService<AgendaPrinter>();
    printer.Print("OpenAPI & Scalar");
}
