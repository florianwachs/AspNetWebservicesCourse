using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Data;
using TechConf.Grpc.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGrpcService<EventGrpcService>();
app.MapGrpcService<SessionGrpcService>();
app.MapGrpcReflectionService();

app.Run();
