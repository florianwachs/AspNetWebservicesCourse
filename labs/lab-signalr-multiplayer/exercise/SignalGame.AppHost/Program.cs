var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.SignalGame_Api>("api");

builder.AddProject<Projects.SignalGame_ReactClient>("react")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
