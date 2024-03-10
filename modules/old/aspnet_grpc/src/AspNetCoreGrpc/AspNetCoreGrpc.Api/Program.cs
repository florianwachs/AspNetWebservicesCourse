using AspNetCoreGrpc.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

//👇 Fügt die benötigten Services hinzu
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);

//👇 CORS Policy, falls der Aufrufer nicht von der gleichen Domain stammt
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//👇 Konfiguration der gRPC-Pipeline
app.UseRouting();
app.UseCors();

//👇 Explizites hinzufügen der gRPC-Web-Unterstützung
app.UseGrpcWeb();

//👇 Einhängen von einem oder mehrerer gRPC Endpunkten, inkl. gRPC-Web-Support
app.UseEndpoints(endpoints =>
{
    app.MapGrpcService<SensorService>().EnableGrpcWeb().RequireCors("AllowAll");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
