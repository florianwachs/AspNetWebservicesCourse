using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<TechConf_Grpc_Server>("api")
    .WithExternalHttpEndpoints();

builder.AddProject<TechConf_Grpc_Client>("producer")
    .WithReference(api)
    .WaitFor(api);

builder.AddViteApp("web", "../TechConf.Grpc.Web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
