var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var tariffsdb = postgres.AddDatabase("tariffsdb");

var server = builder.AddProject<Projects.AspireOpenTelemetry_Server>("server")
    .WithReference(tariffsdb)
    .WaitFor(tariffsdb)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
