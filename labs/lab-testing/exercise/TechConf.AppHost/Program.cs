var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("eventdb");

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddViteApp("web", "../TechConf.Web")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WaitFor(api);

builder.Build().Run();
