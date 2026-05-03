var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("eventdb");

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpsEndpoint();

builder.AddViteApp("web", "../TechConf.Web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
