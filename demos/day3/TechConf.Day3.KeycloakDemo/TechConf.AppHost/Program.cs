var builder = DistributedApplication.CreateBuilder(args);
var keycloakRealmImportPath = Path.Combine(AppContext.BaseDirectory, "Realms");
const string keycloakVolumeName = "day3-keycloak-demo-keycloak";
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("day3-keycloak-demo-postgres")
    .AddDatabase("eventdb");
var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume(keycloakVolumeName)
    .WithRealmImport(keycloakRealmImportPath);

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

builder.Build().Run();
