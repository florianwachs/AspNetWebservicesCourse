using AspNetCoreSignalR.ApiWithSpa.Hubs;
using AspNetCoreSignalR.ApiWithSpa.Jobs;
using AspNetCoreSignalR.ApiWithSpa.Services;
using Coravel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WeatherServices>();
builder.Services.AddScheduler();
builder.Services.AddTransient<CheckForWeatherUpdates>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:44418");
    });
});


var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<CheckForWeatherUpdates>()
        .EveryFifteenSeconds();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("default");

app.MapHub<ChatHub>("/chatHub");
app.MapHub<WeatherHub>("/weatherHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
