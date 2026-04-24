var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WorkshopPlanner_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var web = builder.AddViteApp("web", "../WorkshopPlanner.Web")
    .WithReference(api)
    .WaitFor(api);

api.PublishWithContainerFiles(web, "wwwroot");

builder.Build().Run();
