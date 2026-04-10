var builder = DistributedApplication.CreateBuilder(args);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

// TODO: Task 1 — Add PostgreSQL with a database named "eventdb"
// Hint:
// var postgres = builder.AddPostgres("postgres", password: postgresPassword)
//     .WithDataVolume("lab-keycloak-react-postgres")
//     .AddDatabase("eventdb");

// TODO: Task 2 — Add Keycloak with a named persistent data volume and the imported realm
// Hint:
// const string keycloakVolumeName = "lab-keycloak-react-keycloak";
// var keycloakRealmImportPath = Path.Combine(AppContext.BaseDirectory, "Realms");
// var keycloak = builder.AddKeycloak("keycloak")
//     .WithDataVolume(keycloakVolumeName)
//     .WithRealmImport(keycloakRealmImportPath);

// TODO: Task 2 — Add the API project with references to PostgreSQL and Keycloak
// Hint:
// var api = builder.AddProject<Projects.TechConf_Api>("api")
//      .WithReference(postgres)
//      .WithReference(keycloak)
//      .WaitFor(postgres)
//      .WaitFor(keycloak);


// TODO: Task 6 — Add the React frontend as an npm app
// Hint:
// var webfrontend = builder.AddViteApp("webfrontend", "../techconf-frontend")
//     .WithReference(api)
//     .WithReference(keycloak)
//     .WaitFor(api)
//     .WaitFor(keycloak);
//
// api.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
