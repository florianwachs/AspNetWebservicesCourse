using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Data;
using TechConf.Grpc.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<EventStreamBroadcaster>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGrpcWeb", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

var app = builder.Build();
app.UseCors("AllowGrpcWeb");
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGrpcService<EventGrpcService>()
    .EnableGrpcWeb()
    .RequireCors("AllowGrpcWeb");

// TODO: Task 1 — Map SessionGrpcService after implementing it
// app.MapGrpcService<SessionGrpcService>();

app.MapGrpcReflectionService();
app.MapDefaultEndpoints();

app.Run();
