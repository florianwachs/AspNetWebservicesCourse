using AspNetSignalR.Endpoints;
using AspNetSignalR.Hubs;
using AspNetSignalR.Jobs;
using AspNetSignalR.Services;
using Coravel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
});

// 👇 SignalR Services hinzufügen
builder.Services.AddSignalR();

builder.Services.AddSingleton<WeatherServices>();
builder.Services.AddScheduler();
builder.Services.AddTransient<CheckForWeatherUpdates>();
builder.Services.AddTransient<ScheduledJokesJob>();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:5173");
    });
});

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<CheckForWeatherUpdates>()
        .EveryFifteenSeconds();

    scheduler
        .Schedule<ScheduledJokesJob>()
        .EveryFifteenSeconds();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("default");

// 👇 SignalR Hubs mappen
app.MapHub<ChatHub>("/chatHub");
app.MapHub<WeatherHub>("/weatherHub");

// 👇 API Endpoints mappen
app.MapMyApi();

app.Run();
