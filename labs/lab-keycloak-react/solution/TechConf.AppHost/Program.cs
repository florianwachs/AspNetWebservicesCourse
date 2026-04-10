var builder = DistributedApplication.CreateBuilder(args);

var keycloakRealmImportPath = Path.Combine(AppContext.BaseDirectory, "Realms");
const string keycloakVolumeName = "lab-keycloak-react-keycloak";

// parameters
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("lab-keycloak-react-postgres")
    .AddDatabase("eventdb");

var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume(keycloakVolumeName)
    .WithRealmImport(keycloakRealmImportPath);

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WaitFor(keycloak);

var webfrontend = builder.AddViteApp("webfrontend", "../techconf-frontend")
    .WithReference(api)
    .WithReference(keycloak)
    .WaitFor(api)
    .WaitFor(keycloak);

api.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
