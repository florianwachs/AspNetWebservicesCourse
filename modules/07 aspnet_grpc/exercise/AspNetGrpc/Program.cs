using AspNetGrpc.Services;

var builder = WebApplication.CreateBuilder(args);


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

app.Run();

