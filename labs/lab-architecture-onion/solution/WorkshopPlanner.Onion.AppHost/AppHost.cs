using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<WorkshopPlanner_Api>("api")
    .WithExternalHttpEndpoints();
builder.Build().Run();
