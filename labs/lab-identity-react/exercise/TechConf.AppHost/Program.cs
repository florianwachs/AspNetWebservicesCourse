var builder = DistributedApplication.CreateBuilder(args);

// TODO: Task 1 — Add PostgreSQL with a database named "eventdb"
// Hint:
//var postgres = builder.AddPostgres("postgres")
//.AddDatabase("eventdb");

// TODO: Task 1 — Add the API project with a reference to PostgreSQL
// Hint:
// var api = builder.AddProject<Projects.TechConf_Api>("api")
//      .WithReference(postgres)
//      .WaitFor(postgres);

// TODO: Task 6 — Add the React frontend as an npm app
// Hint:
// var webfrontend = builder.AddViteApp("webfrontend", "../techconf-frontend")
//      .WithReference(api)
//      .WaitFor(api);
//
// api.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();