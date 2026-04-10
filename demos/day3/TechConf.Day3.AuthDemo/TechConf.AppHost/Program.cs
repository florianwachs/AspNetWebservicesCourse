var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("day3-auth-demo-postgres")
    .AddDatabase("eventdb");
var cache = builder.AddRedis("cache");

builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WithReference(cache)
    .WaitFor(postgres)
    //.WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.Build().Run();
