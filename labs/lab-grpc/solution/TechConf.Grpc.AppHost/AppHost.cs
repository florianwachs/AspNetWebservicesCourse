using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var api = builder.AddProject<TechConf_Grpc_Server>("api").WithExternalHttpEndpoints();
builder.Build().Run();
