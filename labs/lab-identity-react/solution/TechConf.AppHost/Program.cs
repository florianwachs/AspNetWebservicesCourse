var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").AddDatabase("eventdb");

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

var webfrontend = builder.AddViteApp("webfrontend", "../techconf-frontend")
    .WithReference(api)
    .WaitFor(api);

api.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
