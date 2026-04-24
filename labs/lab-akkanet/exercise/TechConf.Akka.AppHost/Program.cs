using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<TechConf_Akka_Api>("api")
    .WithExternalHttpEndpoints();

builder.AddViteApp("web", "../TechConf.Akka.Web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
